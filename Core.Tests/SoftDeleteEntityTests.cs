namespace Core.Tests;

/// <summary>
/// Unit tests for <see cref="ISoftDeleteEntity"/> soft deletion and restoration behavior.
/// </summary>
public class SoftDeleteEntityTests
{
    [Test]
    public async Task NewEntity_IsNotDeleted()
    {
        var entity = TestSoftDeleteEntity.Create(Guid.NewGuid());

        await Assert.That(entity.IsDeleted).IsFalse();
        await Assert.That(entity.DeletedAt).IsNull();
        await Assert.That(entity.DeletedBy).IsNull();
    }

    [Test]
    public async Task SoftDelete_MarksEntityAsDeleted()
    {
        var entity = TestSoftDeleteEntity.Create(Guid.NewGuid());

        entity.SoftDelete("admin");

        await Assert.That(entity.IsDeleted).IsTrue();
        await Assert.That(entity.DeletedAt).IsNotNull();
        await Assert.That(entity.DeletedBy).IsEqualTo("admin");
    }

    [Test]
    public async Task SoftDelete_WithNullUser_SetsDeletedByToNull()
    {
        var entity = TestSoftDeleteEntity.Create(Guid.NewGuid());

        entity.SoftDelete();

        await Assert.That(entity.IsDeleted).IsTrue();
        await Assert.That(entity.DeletedBy).IsNull();
    }

    [Test]
    public async Task Restore_ClearsDeletionMetadata()
    {
        var entity = TestSoftDeleteEntity.Create(Guid.NewGuid());
        entity.SoftDelete("admin");

        entity.Restore();

        await Assert.That(entity.IsDeleted).IsFalse();
        await Assert.That(entity.DeletedAt).IsNull();
        await Assert.That(entity.DeletedBy).IsNull();
    }

    [Test]
    public async Task Restore_WhenNotDeleted_RemainsNotDeleted()
    {
        var entity = TestSoftDeleteEntity.Create(Guid.NewGuid());

        entity.Restore();

        await Assert.That(entity.IsDeleted).IsFalse();
    }

    private sealed class TestSoftDeleteEntity : Entity<Guid>, ISoftDeleteEntity
    {
        public bool IsDeleted { get; private set; }
        public DateTimeOffset? DeletedAt { get; private set; }
        public string? DeletedBy { get; private set; }

        public void SoftDelete(string? deletedBy = null)
        {
            IsDeleted = true;
            DeletedAt = DateTimeOffset.UtcNow;
            DeletedBy = deletedBy;
        }

        public void Restore()
        {
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
        }

        public static TestSoftDeleteEntity Create(Guid id) => new() { Id = id };
    }
}

