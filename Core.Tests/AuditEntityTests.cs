namespace Core.Tests;

/// <summary>
/// Unit tests for <see cref="IAuditEntity"/> creation and modification audit tracking.
/// </summary>
public class AuditEntityTests
{
    private const string User1 = "user-1";
    private const string User2 = "user-2";

    [Test]
    public async Task SetCreatedAt_SetsCreationFields()
    {
        var entity = TestAuditableEntity.Create(Guid.NewGuid());
        var timestamp = DateTimeOffset.UtcNow;

        entity.SetCreatedAt(timestamp, User1);

        await Assert.That(entity.CreatedAt).IsEqualTo(timestamp);
        await Assert.That(entity.CreatedBy).IsEqualTo(User1);
    }

    [Test]
    public async Task SetCreatedAt_WithNullUser_SetsTimestampAndNullUser()
    {
        var entity = TestAuditableEntity.Create(Guid.NewGuid());
        var timestamp = DateTimeOffset.UtcNow;

        entity.SetCreatedAt(timestamp, null);

        await Assert.That(entity.CreatedAt).IsEqualTo(timestamp);
        await Assert.That(entity.CreatedBy).IsNull();
    }

    [Test]
    public async Task SetLastModifiedBy_UpdatesModificationFields()
    {
        var entity = TestAuditableEntity.Create(Guid.NewGuid());
        var timestamp = DateTimeOffset.UtcNow;

        entity.SetLastModifiedBy(timestamp, User2);

        await Assert.That(entity.LastModifiedAt).IsEqualTo(timestamp);
        await Assert.That(entity.LastModifiedBy).IsEqualTo(User2);
    }

    [Test]
    public async Task SetLastModifiedBy_WithNullUser_SetsTimestampAndNullUser()
    {
        var entity = TestAuditableEntity.Create(Guid.NewGuid());
        var timestamp = DateTimeOffset.UtcNow;

        entity.SetLastModifiedBy(timestamp, null);

        await Assert.That(entity.LastModifiedAt).IsEqualTo(timestamp);
        await Assert.That(entity.LastModifiedBy).IsNull();
    }

    [Test]
    public async Task NewEntity_HasDefaultAuditFields()
    {
        var entity = TestAuditableEntity.Create(Guid.NewGuid());

        await Assert.That(entity.CreatedAt).IsEqualTo(default(DateTimeOffset));
        await Assert.That(entity.CreatedBy).IsNull();
        await Assert.That(entity.LastModifiedAt).IsNull();
        await Assert.That(entity.LastModifiedBy).IsNull();
    }

    [Test]
    public async Task SetLastModifiedBy_OverwritesPreviousModification()
    {
        var entity = TestAuditableEntity.Create(Guid.NewGuid());
        var first = DateTimeOffset.UtcNow;
        var second = first.AddMinutes(5);

        entity.SetLastModifiedBy(first, User1);
        entity.SetLastModifiedBy(second, User2);

        await Assert.That(entity.LastModifiedAt).IsEqualTo(second);
        await Assert.That(entity.LastModifiedBy).IsEqualTo(User2);
    }

    /// <summary>
    /// Concrete entity implementing <see cref="IAuditEntity"/> for testing audit semantics.
    /// </summary>
    private sealed class TestAuditableEntity : Entity<Guid>, IAuditEntity
    {
        public DateTimeOffset CreatedAt { get; private set; }
        public string? CreatedBy { get; private set; }
        public DateTimeOffset? LastModifiedAt { get; private set; }
        public string? LastModifiedBy { get; private set; }

        public void SetCreatedAt(DateTimeOffset at, string? by)
        {
            CreatedAt = at;
            CreatedBy = by;
        }

        public void SetLastModifiedBy(DateTimeOffset at, string? by)
        {
            LastModifiedAt = at;
            LastModifiedBy = by;
        }

        public static TestAuditableEntity Create(Guid id) =>
            new() { Id = id };
    }
}
