namespace Core.Tests;

/// <summary>
/// Unit tests for <see cref="DomainEvent"/> and <see cref="IDomainEvent"/> behavior.
/// </summary>
public class DomainEventTests
{
    [Test]
    public async Task EventId_IsNonEmpty()
    {
        var domainEvent = new OrderPlacedEvent(Guid.NewGuid(), 99.99m);

        await Assert.That(domainEvent.EventId).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task OccurredOn_IsCloseToCurrentTime()
    {
        var before = DateTimeOffset.UtcNow;
        var domainEvent = new OrderPlacedEvent(Guid.NewGuid(), 50m);
        var after = DateTimeOffset.UtcNow;

        await Assert.That(domainEvent.OccurredOn).IsGreaterThanOrEqualTo(before);
        await Assert.That(domainEvent.OccurredOn).IsLessThanOrEqualTo(after);
    }

    [Test]
    public async Task TwoEvents_HaveDistinctEventIds()
    {
        var event1 = new OrderPlacedEvent(Guid.NewGuid(), 10m);
        var event2 = new OrderPlacedEvent(Guid.NewGuid(), 20m);

        await Assert.That(event1.EventId).IsNotEqualTo(event2.EventId);
    }

    [Test]
    public async Task EventId_CanBeOverriddenViaInit()
    {
        var customId = Guid.NewGuid();
        var domainEvent = new OrderPlacedEvent(Guid.NewGuid(), 100m) { EventId = customId };

        await Assert.That(domainEvent.EventId).IsEqualTo(customId);
    }

    [Test]
    public async Task OccurredOn_CanBeOverriddenViaInit()
    {
        var customTime = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var domainEvent = new OrderPlacedEvent(Guid.NewGuid(), 100m) { OccurredOn = customTime };

        await Assert.That(domainEvent.OccurredOn).IsEqualTo(customTime);
    }

    [Test]
    public async Task RecordEquality_SameValues_AreEqual()
    {
        var orderId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var occurredOn = DateTimeOffset.UtcNow;

        var event1 = new OrderPlacedEvent(orderId, 50m) { EventId = eventId, OccurredOn = occurredOn };
        var event2 = new OrderPlacedEvent(orderId, 50m) { EventId = eventId, OccurredOn = occurredOn };

        await Assert.That(event1).IsEqualTo(event2);
    }

    [Test]
    public async Task RecordEquality_DifferentEventIds_AreNotEqual()
    {
        var orderId = Guid.NewGuid();
        var occurredOn = DateTimeOffset.UtcNow;

        var event1 = new OrderPlacedEvent(orderId, 50m) { EventId = Guid.NewGuid(), OccurredOn = occurredOn };
        var event2 = new OrderPlacedEvent(orderId, 50m) { EventId = Guid.NewGuid(), OccurredOn = occurredOn };

        await Assert.That(event1).IsNotEqualTo(event2);
    }

    private sealed record OrderPlacedEvent(Guid OrderId, decimal Total) : DomainEvent;
}

