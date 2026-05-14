namespace Core;

/// <summary>
/// Scopes an entity to a specific tenant for multi-tenant applications.
/// </summary>
/// <remarks>
/// <para>
/// Implement this interface on entities that must be isolated by tenant.
/// Data access layers should automatically filter queries by <see cref="TenantId"/>
/// to enforce tenant boundaries.
/// </para>
/// </remarks>
/// <seealso cref="Entity{TIdentifier}"/>
public interface ITenantEntity
{
    /// <summary>
    /// Gets the identifier of the tenant that owns this entity.
    /// </summary>
    /// <value>A <see cref="Guid"/> representing the owning tenant.</value>
    Guid TenantId { get; }
}