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
/// <example>
/// Implement tenant isolation on a domain entity:
/// <code>
/// public class Project : Entity&lt;Guid&gt;, ITenantEntity
/// {
///     public Guid TenantId { get; private init; }
///     public string Name { get; init; } = string.Empty;
///
///     public static Project Create(Guid tenantId, string name) =&gt;
///         new() { Id = Guid.NewGuid(), TenantId = tenantId, Name = name };
/// }
/// </code>
/// </example>
/// <seealso cref="Entity{TIdentifier}"/>
public interface ITenantEntity
{
    /// <summary>
    /// Gets the identifier of the tenant that owns this entity.
    /// </summary>
    /// <value>A <see cref="Guid"/> representing the owning tenant.</value>
    Guid TenantId { get; }
}