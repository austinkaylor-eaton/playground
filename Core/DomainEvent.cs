namespace Core;

/// <summary>
/// Base record for domain events, providing default <see cref="EventId"/> and <see cref="OccurredOn"/>.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <inheritdoc/>
    public Guid EventId { get; init; } = Guid.CreateVersion7();

    /// <inheritdoc/>
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}