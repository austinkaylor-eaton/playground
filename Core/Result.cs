namespace Core;

/// <summary>
/// Represents the outcome of a domain operation that may fail with an error.
/// </summary>
/// <typeparam name="T">The type of the successful result value.</typeparam>
public sealed record Result<T>
{
    /// <summary>
    /// The successful result value, or null if the operation failed. <br/>
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// The error message if the operation failed, or null if it succeeded. <br/>
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess => Error is null;

    /// <summary>
    /// Private constructor to enforce use of factory methods. <br/>
    /// </summary>
    /// <param name="value">The successful result value.</param>
    /// <param name="error">The error message if the operation failed.</param>
    private Result(T? value, string? error) => (Value, Error) = (value, error);

    /// <summary>
    /// Creates a successful result with the given value. <br/>
    /// </summary>
    /// <param name="value">The successful result value.</param>
    /// <returns>A <see cref="Result{T}"/> representing a successful operation.</returns>
    public static Result<T> Success(T value) => new(value, null);

    /// <summary>
    /// Creates a failed result with the given error message. <br/>
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A <see cref="Result{T}"/> representing a failed operation.</returns>
    public static Result<T> Failure(string error) => new(default, error);
}