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
}