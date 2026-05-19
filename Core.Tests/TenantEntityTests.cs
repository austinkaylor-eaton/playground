namespace Core.Tests;

/// <summary>
/// Unit tests for <see cref="ITenantEntity"/> tenant scoping behavior.
/// </summary>
public class TenantEntityTests
{
    [Test]
    public async Task TenantId_RetainsAssignedValue()
    {
        var tenantId = Guid.NewGuid();
        var entity = TestTenantEntity.Create(tenantId);

        await Assert.That(entity.TenantId).IsEqualTo(tenantId);
    }

    [Test]
    public async Task TwoEntities_DifferentTenants_HaveDifferentTenantIds()
    {
        var entity1 = TestTenantEntity.Create(Guid.NewGuid());
        var entity2 = TestTenantEntity.Create(Guid.NewGuid());

        await Assert.That(entity1.TenantId).IsNotEqualTo(entity2.TenantId);
    }

    [Test]
    public async Task TenantId_DefaultGuid_IsPermitted()
    {
        var entity = TestTenantEntity.Create(Guid.Empty);

        await Assert.That(entity.TenantId).IsEqualTo(Guid.Empty);
    }

    private sealed class TestTenantEntity : Entity<Guid>, ITenantEntity
    {
        public Guid TenantId { get; private init; }

        public static TestTenantEntity Create(Guid tenantId) =>
            new() { Id = Guid.NewGuid(), TenantId = tenantId };
    }
}

