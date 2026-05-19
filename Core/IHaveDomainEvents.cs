namespace Core;

/// <summary>
/// Adds domain event support to an entity, enabling event-driven patterns.
/// </summary>
/// <remarks>
/// <para>
/// Entities implementing this interface can raise domain events that are collected and dispatched
/// after the unit of work is committed, ensuring consistency between state changes and event publication.
/// </para>
/// </remarks>
/// <example>
/// Raise and clear domain events from an aggregate root:
/// <code>
/// public class Order : Entity&lt;Guid&gt;, IHaveDomainEvents
/// {
///     private readonly List&lt;IDomainEvent&gt; _domainEvents = [];
///
///     public IReadOnlyList&lt;IDomainEvent&gt; DomainEvents =&gt; _domainEvents.AsReadOnly();
///
///     public void ClearDomainEvents() =&gt; _domainEvents.Clear();
///
///     public void Place()
///     {
///         // ... business logic ...
///         _domainEvents.Add(new OrderPlacedEvent(Id));
///     }
/// }
/// </code>
/// </example>
/// <seealso cref="IDomainEvent"/>
/// <seealso cref="DomainEvent"/>
public interface IHaveDomainEvents
{
    /// <summary>
    /// Gets the list of domain events that have been raised by this entity but not yet dispatched.
    /// </summary>
    /// <value>A read-only collection of pending <see cref="IDomainEvent"/> instances.</value>
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears all pending domain events.
    /// </summary>
    /// <remarks>
    /// Called after successful dispatch to prevent duplicate processing.
    /// </remarks>
    void ClearDomainEvents();
}