namespace Core.Tests;

/// <summary>
/// Unit tests for <see cref="IVersionedEntity"/> optimistic concurrency token behavior.
/// </summary>
public class VersionedEntityTests
{
    [Test]
    public async Task RowVersion_RetainsAssignedValue()
    {
        byte[] versionBytes = [1, 2, 3, 4, 5, 6, 7, 8];
        var entity = TestVersionedEntity.Create(Guid.NewGuid(), versionBytes);

        await Assert.That(entity.RowVersion.ToArray()).IsEquivalentTo(versionBytes);
    }

    [Test]
    public async Task RowVersion_DefaultEntity_IsEmpty()
    {
        var entity = TestVersionedEntity.Create(Guid.NewGuid(), []);

        await Assert.That(entity.RowVersion.Length).IsEqualTo(0);
    }

    [Test]
    public async Task RowVersion_DifferentValues_AreNotEqual()
    {
        byte[] version1 = [1, 0, 0, 0, 0, 0, 0, 1];
        byte[] version2 = [1, 0, 0, 0, 0, 0, 0, 2];
        var entity1 = TestVersionedEntity.Create(Guid.NewGuid(), version1);
        var entity2 = TestVersionedEntity.Create(Guid.NewGuid(), version2);

        await Assert.That(entity1.RowVersion.ToArray()).IsNotEquivalentTo(entity2.RowVersion.ToArray());
    }

    private sealed class TestVersionedEntity : Entity<Guid>, IVersionedEntity
    {
        public ReadOnlyMemory<byte> RowVersion { get; private init; }

        public static TestVersionedEntity Create(Guid id, byte[] rowVersion) =>
            new() { Id = id, RowVersion = new ReadOnlyMemory<byte>(rowVersion) };
    }
}

