namespace Core.Validation;

/// <summary>
/// Defines a validator for instances of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type to validate.</typeparam>
/// <example>
/// <code>
/// public class CreateOrderValidator : IValidator&lt;CreateOrderCommand&gt;
/// {
///     public Task&lt;IReadOnlyList&lt;ValidationFailure&gt;&gt; ValidateAsync(
///         CreateOrderCommand instance, CancellationToken cancellationToken)
///     {
///         var failures = new List&lt;ValidationFailure&gt;();
///         if (string.IsNullOrWhiteSpace(instance.ProductName))
///             failures.Add(new ValidationFailure(nameof(instance.ProductName), "Product name is required."));
///         return Task.FromResult&lt;IReadOnlyList&lt;ValidationFailure&gt;&gt;(failures);
///     }
/// }
/// </code>
/// </example>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the specified instance asynchronously.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of validation failures. Empty if valid.</returns>
    Task<IReadOnlyList<ValidationFailure>> ValidateAsync(T instance, CancellationToken cancellationToken);
}

