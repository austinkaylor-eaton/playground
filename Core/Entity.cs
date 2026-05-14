using System.Diagnostics.CodeAnalysis;

namespace Core;

/// <summary>
/// Base domain entity with a typed identifier.
/// </summary>
/// <typeparam name="TIdentifier">The type of the entity's identifier.</typeparam>
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