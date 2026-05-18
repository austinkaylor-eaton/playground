namespace Core.Tests;

/// <summary>
/// Unit tests for <see cref="Entity{TIdentifier}"/> identity-based equality semantics.
/// </summary>
public class EntityTests
{
    private const string NameAlice = "Alice";
    private const string NameBob = "Bob";

    [Test]
    public async Task Equals_SameTypeAndId_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var entity1 = TestEntity.Create(id, NameAlice);
        var entity2 = TestEntity.Create(id, NameBob);

        await Assert.That(entity1.Equals(entity2)).IsTrue();
        await Assert.That(entity1 == entity2).IsTrue();
    }

    [Test]
    public async Task Equals_SameTypeDifferentId_ReturnsFalse()
    {
        var entity1 = TestEntity.Create(Guid.NewGuid(), NameAlice);
        var entity2 = TestEntity.Create(Guid.NewGuid(), NameAlice);

        await Assert.That(entity1.Equals(entity2)).IsFalse();
        await Assert.That(entity1 != entity2).IsTrue();
    }

    [Test]
    public async Task Equals_DifferentTypeSameId_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        var entity1 = TestEntity.Create(id, NameAlice);
        var entity2 = OtherTestEntity.Create(id);

        await Assert.That(entity1.Equals(entity2)).IsFalse();
    }

    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "CA1508:Avoid dead conditional code",
        Justification = "Intentionally testing null equality behavior")]
    public async Task Equals_Null_ReturnsFalse()
    {
        var entity = TestEntity.Create(Guid.NewGuid(), NameAlice);

        await Assert.That(entity.Equals(null)).IsFalse();
        await Assert.That(entity == null).IsFalse();
    }

    [Test]
    public async Task GetHashCode_SameTypeAndId_AreEqual()
    {
        var id = Guid.NewGuid();
        var entity1 = TestEntity.Create(id, NameAlice);
        var entity2 = TestEntity.Create(id, NameBob);

        await Assert.That(entity1.GetHashCode()).IsEqualTo(entity2.GetHashCode());
    }

    [Test]
    public async Task ToString_ReturnsTypeNameAndId()
    {
        var id = Guid.NewGuid();
        var entity = TestEntity.Create(id, NameAlice);

        await Assert.That(entity.ToString()).IsEqualTo($"TestEntity [Id={id}]");
    }

    /// <summary>
    /// Concrete entity used for testing equality and identity semantics.
    /// </summary>
    private sealed class TestEntity : Entity<Guid>
    {
        public string Name { get; private init; } = string.Empty;

        public static TestEntity Create(Guid id, string name) =>
            new() { Id = id, Name = name };
    }

    /// <summary>
    /// A different entity type to verify cross-type equality returns false.
    /// </summary>
    private sealed class OtherTestEntity : Entity<Guid>
    {
        public static OtherTestEntity Create(Guid id) =>
            new() { Id = id };
    }
}