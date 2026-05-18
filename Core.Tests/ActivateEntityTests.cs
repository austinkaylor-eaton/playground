namespace Core.Tests;

/// <summary>
/// Unit tests for <see cref="IActivateEntity"/> activation and deactivation behavior.
/// </summary>
public class ActivateEntityTests
{
    [Test]
    public async Task Activate_SetsIsActiveToTrue()
    {
        var entity = TestActivatableEntity.Create(Guid.NewGuid());

        entity.Activate();

        await Assert.That(entity.IsActive).IsTrue();
    }

    [Test]
    public async Task Deactivate_SetsIsActiveToFalse()
    {
        var entity = TestActivatableEntity.Create(Guid.NewGuid());
        entity.Activate();

        entity.Deactivate();

        await Assert.That(entity.IsActive).IsFalse();
    }

    [Test]
    public async Task NewEntity_IsInactiveByDefault()
    {
        var entity = TestActivatableEntity.Create(Guid.NewGuid());

        await Assert.That(entity.IsActive).IsFalse();
    }

    [Test]
    public async Task Activate_CalledMultipleTimes_RemainsActive()
    {
        var entity = TestActivatableEntity.Create(Guid.NewGuid());

        entity.Activate();
        entity.Activate();

        await Assert.That(entity.IsActive).IsTrue();
    }

    [Test]
    public async Task Deactivate_WhenAlreadyInactive_RemainsInactive()
    {
        var entity = TestActivatableEntity.Create(Guid.NewGuid());

        entity.Deactivate();

        await Assert.That(entity.IsActive).IsFalse();
    }

    /// <summary>
    /// Concrete entity implementing <see cref="IActivateEntity"/> for testing activation semantics.
    /// </summary>
    private sealed class TestActivatableEntity : Entity<Guid>, IActivateEntity
    {
        public bool IsActive { get; private set; }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        public static TestActivatableEntity Create(Guid id) =>
            new() { Id = id };
    }
}

