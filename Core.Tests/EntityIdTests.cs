namespace Core.Tests;

/// <summary>
/// Unit tests for <see cref="IEntityId{T}"/> strongly-typed identifier behavior.
/// </summary>
public class EntityIdTests
{
    [Test]
    public async Task Value_ReturnsUnderlyingGuid()
    {
        var guid = Guid.NewGuid();
        var orderId = new OrderId(guid);

        await Assert.That(orderId.Value).IsEqualTo(guid);
    }

    [Test]
    public async Task Value_ReturnsUnderlyingInt()
    {
        var productId = new ProductId(42);

        await Assert.That(productId.Value).IsEqualTo(42);
    }

    [Test]
    public async Task TwoIds_SameValue_AreEqual()
    {
        var guid = Guid.NewGuid();
        var id1 = new OrderId(guid);
        var id2 = new OrderId(guid);

        await Assert.That(id1).IsEqualTo(id2);
    }

    [Test]
    public async Task TwoIds_DifferentValues_AreNotEqual()
    {
        var id1 = new OrderId(Guid.NewGuid());
        var id2 = new OrderId(Guid.NewGuid());

        await Assert.That(id1).IsNotEqualTo(id2);
    }

    [Test]
    public async Task DifferentIdTypes_SameUnderlyingValue_AreNotInterchangeable()
    {
        var guid = Guid.NewGuid();
        var orderId = new OrderId(guid);
        var customerId = new CustomerId(guid);

        await Assert.That(orderId.Value).IsEqualTo(customerId.Value);
        await Assert.That(orderId.GetType()).IsNotEqualTo(customerId.GetType());
    }

    [Test]
    public async Task DefaultId_HasDefaultValue()
    {
        var orderId = default(OrderId);

        await Assert.That(orderId.Value).IsEqualTo(Guid.Empty);
    }

    [Test]
    public async Task EntityWithStronglyTypedId_UsesIdCorrectly()
    {
        var guid = Guid.NewGuid();
        var orderId = new OrderId(guid);
        var entity = TestOrder.Create(orderId);

        await Assert.That(entity.Id).IsEqualTo(orderId);
        await Assert.That(entity.Id.Value).IsEqualTo(guid);
    }

    private readonly record struct OrderId(Guid Value) : IEntityId<Guid>;

    private readonly record struct CustomerId(Guid Value) : IEntityId<Guid>;

    private readonly record struct ProductId(int Value) : IEntityId<int>;

    private sealed class TestOrder : Entity<OrderId>
    {
        public static TestOrder Create(OrderId id) => new() { Id = id };
    }
}

