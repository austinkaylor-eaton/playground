namespace Core;

/// <summary>
/// Marker interface for strongly-typed entity identifiers.
/// </summary>
/// <typeparam name="T">The underlying primitive type of the identifier (e.g., <see cref="Guid"/>, <see cref="int"/>).</typeparam>
/// <remarks>
/// Implement this interface as a <c>readonly record struct</c> to create strongly-typed IDs
/// that prevent accidental misuse of raw primitives across different entity types.
/// </remarks>
/// <example>
/// Define a strongly-typed identifier for an Order entity:
/// <code>
/// public readonly record struct OrderId(Guid Value) : IEntityId&lt;Guid&gt;;
/// </code>
/// </example>
/// <seealso cref="Entity{TIdentifier}"/>
public interface IEntityId<out T> where T : notnull
{
    /// <summary>
    /// Gets the underlying primitive value of the identifier.
    /// </summary>
    /// <value>The raw <typeparamref name="T"/> value that uniquely identifies the entity.</value>
    T Value { get; }
}