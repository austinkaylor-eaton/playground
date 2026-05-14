namespace Core.Validation;

/// <summary>
/// Represents a single validation failure for a property.
/// </summary>
/// <param name="PropertyName">The name of the property that failed validation.</param>
/// <param name="ErrorMessage">The error message describing the failure.</param>
/// <example>
/// <code>
/// var failure = new ValidationFailure("Email", "Email is required.");
/// </code>
/// </example>
public sealed record ValidationFailure(string PropertyName, string ErrorMessage);

