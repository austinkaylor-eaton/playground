namespace Core;

/// <summary>
/// Scopes an entity to a specific tenant for multi-tenant applications.
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// The identifier of the tenant that owns this entity.
    /// </summary>
    Guid TenantId { get; }
}