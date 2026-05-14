namespace Core;

/// <summary>
/// Adds domain event support to an entity, enabling event-driven patterns.
/// </summary>
public interface IHaveDomainEvents
{
    /// <summary>
    /// Gets the list of domain events that have been raised by this entity but not yet dispatched.
    /// </summary>
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears all pending domain events. <br/>
    /// Called after successful dispatch.
    /// </summary>
    void ClearDomainEvents();
}