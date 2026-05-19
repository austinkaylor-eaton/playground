using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Core;

/// <summary>
/// Represents the result of an operation, encapsulating either a successful value or an error message.
/// </summary>
/// <typeparam name="TSuccess">The type of the successful result value.</typeparam>
/// <typeparam name="TError">The type of the error, must derive from <see cref="ResultError"/>.</typeparam>
[PublicAPI]
public readonly record struct Result<TSuccess, TError> where TError : ResultError
{
    /// <summary>
    /// The value of the successful result. Will be null if the operation failed.
    /// </summary>
    public TSuccess? Value { get; }

    /// <summary>
    /// The error message if the operation failed. Will be null if the operation succeeded.
    /// </summary>
    public TError? Error { get; }

    /// <summary>
    /// Indicates whether the operation was successful.
    /// A default-constructed Result is considered a failure.
    /// </summary>
    public bool IsSuccess => Error is null;

    /// <summary>
    /// Creates a successful result with the given value.
    /// </summary>
    /// <param name="value">The value of the successful result.</param>
    /// <param name="error">The error message if the operation failed.</param>
    /// <exception cref="InvalidOperationException">Thrown if both <paramref name="value"/> and <paramref name="error"/> are non-null.</exception>
    internal Result(TSuccess? value, TError? error)
    {
        if (value is not null && error is not null)
        {
            throw new InvalidOperationException("A result cannot have both a value and an error.");
        }

        Value = value;
        Error = error;
    }
}

/// <summary>
/// Factory methods for creating <see cref="Result{TSuccess, TError}"/> instances.
/// </summary>
[PublicAPI]
[SuppressMessage("Design", "CA1034: Nested types should not be visible", Justification = "Extension members are part of C# 14+ https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods")]
[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
public static class Result
{
    /// <summary>
    /// Creates a successful result with the given value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the successful result value.</typeparam>
    /// <typeparam name="TError">The type of the error, must derive from <see cref="ResultError"/>.</typeparam>
    /// <param name="value">The successful result value.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    /// <returns>A <see cref="Result{TSuccess, TError}"/> representing a successful operation.</returns>
    public static Result<TSuccess, TError> Success<TSuccess, TError>(TSuccess value)
        where TError : ResultError =>
        value is null
            ? throw new ArgumentNullException(nameof(value), "A successful result must have a non-null value.")
            : new Result<TSuccess, TError>(value, null);

    /// <summary>
    /// Creates a failed result with the given error.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the expected result value.</typeparam>
    /// <typeparam name="TError">The type of the error, must derive from <see cref="ResultError"/>.</typeparam>
    /// <param name="error">The error.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error"/> is null.</exception>
    /// <returns>A <see cref="Result{TSuccess, TError}"/> representing a failed operation.</returns>
    public static Result<TSuccess, TError> Failure<TSuccess, TError>(TError error)
        where TError : ResultError =>
        error is null
            ? throw new ArgumentNullException(nameof(error), "A failed result must have a non-null error.")
            : new Result<TSuccess, TError>(default, error);

    /// <param name="result">The input result to transform.</param>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TError">The type of the error, must derive from <see cref="ResultError"/>.</typeparam>
    extension<TIn, TError>(Result<TIn, TError> result)
        where TError : ResultError
    {
        /// <summary>
        /// Transforms the value of a successful result using the given mapping function.
        /// </summary>
        /// <typeparam name="TOut">The type of the output result value.</typeparam>
        /// <param name="map">The mapping function to apply to the successful result value.</param>
        /// <example>
        /// Simple — extract a single property from a successful result:
        /// <code>
        /// Result&lt;Order, ResultError&gt; orderResult = Result.Success&lt;Order, ResultError&gt;(new Order { Id = 1, TotalAmount = 99.99m });
        /// Result&lt;decimal, ResultError&gt; amountResult = orderResult.Map(order => order.TotalAmount);
        /// // amountResult.IsSuccess == true, amountResult.Value == 99.99m
        /// </code>
        /// <br/>
        /// Advanced — chain multiple Map calls to transform through several types:
        /// <code>
        /// Result&lt;string, ResultError&gt; formatted = Result.Success&lt;Order, ResultError&gt;(new Order { Id = 1, TotalAmount = 99.99m })
        ///     .Map(order => order.TotalAmount)
        ///     .Map(amount => amount * 1.1m)
        ///     .Map(total => $"Total with tax: {total:C}");
        /// // formatted.IsSuccess == true, formatted.Value == "Total with tax: $109.99"
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="map"/> is null.</exception>
        /// <returns>A new <see cref="Result{TOut, TError}"/> containing the mapped value if the input result was successful, or the original error if it was a failure.</returns>
        public Result<TOut, TError> Map<TOut>(Func<TIn, TOut> map)
        {
            ArgumentNullException.ThrowIfNull(map);

            return result.IsSuccess
                ? Success<TOut, TError>(map(result.Value!))
                : Failure<TOut, TError>(result.Error!);
        }
    }

    /// <summary>
    /// Chains a dependent operation that itself returns a Result.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <typeparam name="TError">The type of the error, must derive from <see cref="ResultError"/>.</typeparam>
    /// <param name="result">The input result to chain from.</param>
    /// <param name="bind">The function that takes the successful result value and returns a new Result.</param>
    /// <example>
    /// Simple — chain a validation step that may fail:
    /// <code>
    /// Result&lt;string, ResultError&gt; emailResult = Result.Success&lt;string, ResultError&gt;("user@example.com");
    /// Result&lt;Customer, ResultError&gt; customerResult = emailResult.Bind(email =>
    ///     email.Contains('@')
    ///         ? Result.Success&lt;Customer, ResultError&gt;(new Customer { Email = email })
    ///         : Result.Failure&lt;Customer, ResultError&gt;(new ValidationError("Invalid email format.")));
    /// </code>
    /// <br/>
    /// Advanced — chain multiple dependent operations that each may fail:
    /// <code>
    /// Result&lt;Invoice, ResultError&gt; invoiceResult = Result.Success&lt;Guid, ResultError&gt;(orderId)
    ///     .Bind(id => orderRepository.FindById(id) is { } order
    ///         ? Result.Success&lt;Order, ResultError&gt;(order)
    ///         : Result.Failure&lt;Order, ResultError&gt;(new NotFoundError("Order", id)))
    ///     .Bind(order => invoiceService.Generate(order));
    /// </code>
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="bind"/> is null.</exception>
    /// <returns>The Result returned by the bind function if the input result was successful, or the original error if it was a failure.</returns>
    public static Result<TOut, TError> Bind<TIn, TOut, TError>(this Result<TIn, TError> result, Func<TIn, Result<TOut, TError>> bind)
        where TError : ResultError
    {
        ArgumentNullException.ThrowIfNull(bind);

        return result.IsSuccess
            ? bind(result.Value!)
            : Failure<TOut, TError>(result.Error!);
    }
}