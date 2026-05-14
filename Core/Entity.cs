using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Core;

/// <summary>
/// Base domain entity with a typed identifier.
/// </summary>
/// <typeparam name="TIdentifier">The type of the entity's identifier.</typeparam>
/// <remarks>
/// <para>
/// All domain entities should inherit from this class to gain identity-based equality semantics.
/// Two entities are considered equal if they share the same runtime type and <see cref="Id"/> value.
/// </para>
/// <para>
/// Derived classes must set the <see cref="Id"/> property via the <c>protected init</c> accessor,
/// typically through a constructor or object initializer.
/// </para>
/// </remarks>
/// <example>
/// Define a domain entity with a <see cref="Guid"/> identifier:
/// <code>
/// public class Order : Entity&lt;Guid&gt;
/// {
///     public string CustomerName { get; init; } = string.Empty;
///     public decimal Total { get; init; }
///
///     public static Order Create(string customerName, decimal total) =&gt;
///         new() { Id = Guid.NewGuid(), CustomerName = customerName, Total = total };
/// }
/// </code>
/// </example>
/// <example>
/// Use a strongly-typed identifier:
/// <code>
/// public readonly record struct OrderId(Guid Value) : IEntityId&lt;Guid&gt;;
///
/// public class Order : Entity&lt;OrderId&gt;
/// {
///     public static Order Create(string name) =&gt;
///         new() { Id = new OrderId(Guid.NewGuid()) };
/// }
/// </code>
/// </example>
[PublicAPI]
[SuppressMessage("Major Code Smell", "S4035",
    Justification = "Equality includes runtime type check, safe for inheritance")]
public abstract class Entity<TIdentifier> : IEquatable<Entity<TIdentifier>>
    where TIdentifier : notnull
{
    /// <summary>
    /// Gets the unique identifier for this entity.
    /// </summary>
    /// <value>
    /// The typed identifier that uniquely distinguishes this entity within its aggregate or bounded context.
    /// </value>
    /// <remarks>
    /// Must be set by derived classes, typically via constructor or object initializer.
    /// </remarks>
    public TIdentifier Id { get; protected init; } = default!;

    /// <summary>
    /// Determines whether the specified object is equal to the current entity.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is an <see cref="Entity{TIdentifier}"/> of the same runtime type with an equal <see cref="Id"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        obj is Entity<TIdentifier> other && Equals(other);

    /// <summary>
    /// Determines whether the specified entity is equal to the current entity.
    /// </summary>
    /// <param name="other">The entity to compare with the current entity.</param>
    /// <returns><c>true</c> if <paramref name="other"/> is not <c>null</c>, shares the same runtime type, and has an equal <see cref="Id"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(Entity<TIdentifier>? other) =>
        other is not null && GetType() == other.GetType() && Id.Equals(other.Id);

    /// <summary>
    /// Returns a hash code based on the entity's runtime type and <see cref="Id"/>.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    /// <summary>
    /// Determines whether two <see cref="Entity{TIdentifier}"/> instances are equal.
    /// </summary>
    /// <param name="left">The left-hand entity.</param>
    /// <param name="right">The right-hand entity.</param>
    /// <returns><c>true</c> if both entities are equal or both are <c>null</c>; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Entity<TIdentifier>? left, Entity<TIdentifier>? right) =>
        Equals(left, right);

    /// <summary>
    /// Determines whether two <see cref="Entity{TIdentifier}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The left-hand entity.</param>
    /// <param name="right">The right-hand entity.</param>
    /// <returns><c>true</c> if the entities are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Entity<TIdentifier>? left, Entity<TIdentifier>? right) =>
        !Equals(left, right);

    /// <summary>
    /// Returns a string representation of the entity including its type name and <see cref="Id"/>.
    /// </summary>
    /// <returns>A string in the format <c>TypeName [Id=value]</c>.</returns>
    public override string ToString() => $"{GetType().Name} [Id={Id}]";
}