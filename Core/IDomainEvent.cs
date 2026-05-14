namespace Core;

/// <summary>
/// Marker interface for domain events.
/// </summary>
public interface IDomainEvent
{
        /// <summary>
        /// The date and time when the domain event occurred.
        /// </summary>
        DateTimeOffset OccurredOn { get; }
}