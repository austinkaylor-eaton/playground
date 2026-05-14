namespace Core;

/// <summary>
/// Base record for domain events, providing default <see cref="EventId"/> and <see cref="OccurredOn"/>.
/// </summary>
/// <remarks>
/// <para>
/// Derive from this record to define specific domain events. The <see cref="EventId"/> is generated
/// using a version 7 UUID for time-ordered uniqueness, and <see cref="OccurredOn"/> defaults to the
/// current UTC time.
/// </para>
/// </remarks>
/// <example>
/// Define a custom domain event:
/// <code>
/// public record OrderPlacedEvent(Guid OrderId, decimal Total) : DomainEvent;
/// </code>
/// </example>
/// <seealso cref="IDomainEvent"/>
/// <seealso cref="IHaveDomainEvents"/>
public abstract record DomainEvent : IDomainEvent
{
    /// <inheritdoc/>
    public Guid EventId { get; init; } = Guid.CreateVersion7();

    /// <inheritdoc/>
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}