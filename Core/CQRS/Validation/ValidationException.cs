namespace Core.CQRS.Validation;

/// <summary>
/// Exception thrown when one or more validation failures occur in the CQRS pipeline.
/// </summary>
/// <example>
/// <code>
/// var errors = new[] { new ValidationFailure("Name", "Name is required.") };
/// throw new ValidationException(errors);
/// </code>
/// </example>
public sealed class ValidationException : Exception
{
    /// <summary>
    /// Gets the collection of validation failures that caused this exception.
    /// </summary>
    public IReadOnlyList<ValidationFailure> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    public ValidationException()
        : base("One or more validation failures occurred.")
    {
        Errors = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ValidationException(string message)
        : base(message)
    {
        Errors = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
        Errors = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with validation failures.
    /// </summary>
    /// <param name="errors">The validation failures.</param>
    public ValidationException(IReadOnlyList<ValidationFailure> errors)
        : base("One or more validation failures occurred.")
    {
        Errors = errors ?? [];
    }

    /// <inheritdoc />
    public override string Message =>
        Errors is { Count: > 0 }
            ? $"Validation failed with {Errors.Count} error(s): " +
              string.Join("; ", Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"))
            : base.Message;
}
