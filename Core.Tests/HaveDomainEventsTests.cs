namespace Core.Tests;

/// <summary>
/// Unit tests for <see cref="IHaveDomainEvents"/> domain event collection and clearing behavior.
/// </summary>
public class HaveDomainEventsTests
{
    [Test]
    public async Task DomainEvents_NewEntity_IsEmpty()
    {
        var entity = TestAggregate.Create(Guid.NewGuid());

        await Assert.That(entity.DomainEvents).IsEmpty();
    }

    [Test]
    public async Task DomainEvents_AfterRaisingEvent_ContainsEvent()
    {
        var entity = TestAggregate.Create(Guid.NewGuid());

        entity.Place();

        await Assert.That(entity.DomainEvents).Count().IsEqualTo(1);
    }

    [Test]
    public async Task DomainEvents_AfterRaisingMultipleEvents_ContainsAllEvents()
    {
        var entity = TestAggregate.Create(Guid.NewGuid());

        entity.Place();
        entity.Place();
        entity.Place();

        await Assert.That(entity.DomainEvents).Count().IsEqualTo(3);
    }

    [Test]
    public async Task ClearDomainEvents_RemovesAllPendingEvents()
    {
        var entity = TestAggregate.Create(Guid.NewGuid());
        entity.Place();
        entity.Place();

        entity.ClearDomainEvents();

        await Assert.That(entity.DomainEvents).IsEmpty();
    }

    [Test]
    public async Task ClearDomainEvents_WhenAlreadyEmpty_RemainsEmpty()
    {
        var entity = TestAggregate.Create(Guid.NewGuid());

        entity.ClearDomainEvents();

        await Assert.That(entity.DomainEvents).IsEmpty();
    }

    [Test]
    public async Task DomainEvents_AfterClearAndRaise_ContainsOnlyNewEvents()
    {
        var entity = TestAggregate.Create(Guid.NewGuid());
        entity.Place();
        entity.ClearDomainEvents();

        entity.Place();

        await Assert.That(entity.DomainEvents).Count().IsEqualTo(1);
    }

    [Test]
    public async Task DomainEvents_RaisedEvent_ImplementsIDomainEvent()
    {
        var entity = TestAggregate.Create(Guid.NewGuid());

        entity.Place();

        var domainEvent = entity.DomainEvents[0];
        await Assert.That(domainEvent.EventId).IsNotEqualTo(Guid.Empty);
        await Assert.That(domainEvent.OccurredOn).IsNotEqualTo(default(DateTimeOffset));
    }

    private sealed record OrderPlacedEvent(Guid OrderId) : DomainEvent;

    private sealed class TestAggregate : Entity<Guid>, IHaveDomainEvents
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void ClearDomainEvents() => _domainEvents.Clear();

        public void Place() => _domainEvents.Add(new OrderPlacedEvent(Id));

        public static TestAggregate Create(Guid id) => new() { Id = id };
    }
}



