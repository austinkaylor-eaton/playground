using JetBrains.Annotations;

namespace Core;

/// <summary>
/// Represents the result of an operation, encapsulating either a successful value or an error message.
/// </summary>
/// <typeparam name="T">The type of the successful result value.</typeparam>
[PublicAPI]
public readonly record struct Result<T>
{
    /// <summary>
    /// The value of the successful result. Will be null if the operation failed.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// The error message if the operation failed. Will be null if the operation succeeded.
    /// </summary>
    public string? Error { get; }

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
    internal Result(T? value, string? error)
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
/// Factory methods for creating <see cref="Result{T}"/> instances.
/// </summary>
[PublicAPI]
public static class Result
{
    /// <summary>
    /// Creates a successful result with the given value.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="value">The successful result value.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    /// <returns>A <see cref="Result{T}"/> representing a successful operation.</returns>
    public static Result<T> Success<T>(T value) =>
        value is null
            ? throw new ArgumentNullException(nameof(value), "A successful result must have a non-null value.")
            : new Result<T>(value, null);

    /// <summary>
    /// Creates a failed result with the given error message.
    /// </summary>
    /// <typeparam name="T">The type of the expected result value.</typeparam>
    /// <param name="error">The error message.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="error"/> is null or empty.</exception>
    /// <returns>A <see cref="Result{T}"/> representing a failed operation.</returns>
    public static Result<T> Failure<T>(string error) =>
        string.IsNullOrWhiteSpace(error)
            ? throw new ArgumentException("Error message must not be null or empty.", nameof(error))
            : new Result<T>(default, error);

    /// <summary>
    /// Transforms the value of a successful result using the given mapping function.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="result">The input result to transform.</param>
    /// <param name="map">The mapping function to apply to the successful result value.</param>
    /// <example>
    /// Simple — extract a single property from a successful result:
    /// <code>
    /// Result&lt;Order&gt; orderResult = Result.Success(new Order { Id = 1, TotalAmount = 99.99m });
    /// Result&lt;decimal&gt; amountResult = orderResult.Map(order => order.TotalAmount);
    /// // amountResult.IsSuccess == true, amountResult.Value == 99.99m
    /// </code>
    /// <br/>
    /// Advanced — chain multiple Map calls to transform through several types:
    /// <code>
    /// Result&lt;string&gt; formatted = Result.Success(new Order { Id = 1, TotalAmount = 99.99m })
    ///     .Map(order => order.TotalAmount)
    ///     .Map(amount => amount * 1.1m)
    ///     .Map(total => $"Total with tax: {total:C}");
    /// // formatted.IsSuccess == true, formatted.Value == "Total with tax: $109.99"
    ///
    /// // When the source result is a failure, Map short-circuits and preserves the error:
    /// Result&lt;string&gt; failed = Result.Failure&lt;Order&gt;("Order not found")
    ///     .Map(order => order.TotalAmount)
    ///     .Map(amount => $"Amount: {amount:C}");
    /// // failed.IsSuccess == false, failed.Error == "Order not found"
    /// </code>
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="map"/> is null.</exception>
    /// <returns>A new <see cref="Result{TOut}"/> containing the mapped value if the input result was successful, or the original error if it was a failure.</returns>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map)
    {
        ArgumentNullException.ThrowIfNull(map);

        return result.IsSuccess
            ? Success(map(result.Value!))
            : Failure<TOut>(result.Error!);
    }

    /// <summary>
    /// Chains a dependent operation that itself returns a Result.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="result">The input result to chain from.</param>
    /// <param name="bind">The function that takes the successful result value and returns a new Result.</param>
    /// <example>
    /// Simple — chain a validation step that may fail:
    /// <code>
    /// Result&lt;string&gt; emailResult = Result.Success("user@example.com");
    /// Result&lt;Customer&gt; customerResult = emailResult.Bind(email =>
    ///     email.Contains('@')
    ///         ? Result.Success(new Customer { Email = email })
    ///         : Result.Failure&lt;Customer&gt;("Invalid email format."));
    /// // customerResult.IsSuccess == true, customerResult.Value.Email == "user@example.com"
    /// </code>
    /// <br/>
    /// Advanced — chain multiple dependent operations that each may fail:
    /// <code>
    /// Result&lt;Invoice&gt; invoiceResult = Result.Success(orderId)
    ///     .Bind(id => orderRepository.FindById(id) is { } order
    ///         ? Result.Success(order)
    ///         : Result.Failure&lt;Order&gt;("Order not found."))
    ///     .Bind(order => order.Items.Count > 0
    ///         ? Result.Success(order)
    ///         : Result.Failure&lt;Order&gt;("Order has no items."))
    ///     .Bind(order => invoiceService.Generate(order));
    ///
    /// // If any step fails, the chain short-circuits and preserves that error:
    /// // invoiceResult.IsSuccess == false, invoiceResult.Error == "Order has no items."
    /// </code>
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="bind"/> is null.</exception>
    /// <returns>The Result returned by the bind function if the input result was successful, or the original error if it was a failure.</returns>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> bind)
    {
        ArgumentNullException.ThrowIfNull(bind);

        return result.IsSuccess
            ? bind(result.Value!)
            : Failure<TOut>(result.Error!);
    }
}