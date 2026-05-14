namespace Core;

/// <summary>
/// Marker interface for domain events.
/// </summary>
/// <remarks>
/// Implement this interface (or derive from <see cref="DomainEvent"/>) to define events
/// that capture meaningful state changes within a domain aggregate.
/// </remarks>
/// <seealso cref="DomainEvent"/>
/// <seealso cref="IHaveDomainEvents"/>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for idempotent event processing.
    /// </summary>
    /// <value>A <see cref="Guid"/> that uniquely identifies this event instance.</value>
    Guid EventId { get; }

    /// <summary>
    /// Gets the date and time when the domain event occurred.
    /// </summary>
    /// <value>A <see cref="DateTimeOffset"/> representing the moment the event was raised.</value>
    DateTimeOffset OccurredOn { get; }
}

