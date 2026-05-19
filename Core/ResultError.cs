namespace Core;

/// <summary>
/// Represents an error for a <see cref="Result{TSuccess, TError}"/> in a functional style, encapsulating an error message.
/// </summary>
/// <param name="Message">The error message.</param>
public abstract record ResultError(string Message);