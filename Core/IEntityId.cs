namespace Core;

/// <summary>
/// Marker interface for strongly-typed entity identifiers.
/// </summary>
public interface IEntityId<out T> where T : notnull
{
    T Value { get; }
}