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
    /// <returns>A new <see cref="Result{TOut}"/> containing the mapped value if the input result was successful, or the original error if it was a failure.</returns>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        ArgumentNullException.ThrowIfNull(result.Value);
        ArgumentNullException.ThrowIfNull(result.Error);

        return result.IsSuccess
            ? Success(map(result.Value))
            : Failure<TOut>(result.Error);
    }


    /// <summary>
    /// Chains a dependent operation that itself returns a Result.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="result">The input result to chain from.</param>
    /// <param name="bind">The function that takes the successful result value and returns a new Result.</param>
    /// <returns>The Result returned by the bind function if the input result was successful, or the original error if it was a failure.</returns>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> bind)
    {
        ArgumentNullException.ThrowIfNull(bind);
        ArgumentNullException.ThrowIfNull(result.Value);
        ArgumentNullException.ThrowIfNull(result.Error);

        return result.IsSuccess
            ? bind(result.Value)
            : Failure<TOut>(result.Error);
    }
}