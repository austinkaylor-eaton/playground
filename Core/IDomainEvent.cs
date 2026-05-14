namespace Core;

/// <summary>
/// Marker interface for domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Unique identifier for idempotent event processing.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// The date and time when the domain event occurred.
    /// </summary>
    DateTimeOffset OccurredOn { get; }
}