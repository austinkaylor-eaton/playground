using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Core;

/// <summary>
/// Base domain entity with a typed identifier.
/// </summary>
/// <typeparam name="TIdentifier">The type of the entity's identifier.</typeparam>
/// <example>
/// Define a domain entity with a <see cref="Guid"/> identifier:
/// <code>
/// public class Order : Entity&lt;Guid&gt;
/// {
///     public string CustomerName { get; init; } = string.Empty;
///     public decimal Total { get; init; }
///
///     public static Order Create(string customerName, decimal total) =>
///         new() { Id = Guid.NewGuid(), CustomerName = customerName, Total = total };
/// }
/// </code>
/// </example>
/// <example>
/// Use a strongly-typed identifier with <see cref="IEntityId{T}"/>:
/// <code>
/// public readonly record struct OrderId(Guid Value) : IEntityId&lt;Guid&gt;;
///
/// public class Order : Entity&lt;OrderId&gt;
/// {
///     public string CustomerName { get; init; } = string.Empty;
///
///     public static Order Create(string customerName) =>
///         new() { Id = new OrderId(Guid.NewGuid()), CustomerName = customerName };
/// }
/// </code>
/// </example>
/// <example>
/// Entity equality is based on type and identifier:
/// <code>
/// var order1 = Order.Create("Alice", 99.99m);
/// var order2 = Order.Create("Bob", 50.00m);
///
/// // Same reference
/// bool same = order1 == order1;    // true
///
/// // Different identifiers
/// bool different = order1 == order2; // false
/// </code>
/// </example>
[PublicAPI]
[SuppressMessage("Major Code Smell", "S4035",
    Justification = "Equality includes runtime type check, safe for inheritance")]
public abstract class Entity<TIdentifier> : IEquatable<Entity<TIdentifier>>
    where TIdentifier : notnull
{
    /// <summary>
    /// The unique identifier for this entity.
    /// </summary>
    /// <remarks>
    /// Must be set by derived classes, typically via constructor or initializer.
    /// </remarks>
    public TIdentifier Id { get; protected init; } = default!;

    public override bool Equals(object? obj) =>
        obj is Entity<TIdentifier> other && Equals(other);

    public bool Equals(Entity<TIdentifier>? other) =>
        other is not null && GetType() == other.GetType() && Id.Equals(other.Id);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(Entity<TIdentifier>? left, Entity<TIdentifier>? right) =>
        Equals(left, right);

    public static bool operator !=(Entity<TIdentifier>? left, Entity<TIdentifier>? right) =>
        !Equals(left, right);

    public override string ToString() => $"{GetType().Name} [Id={Id}]";
}