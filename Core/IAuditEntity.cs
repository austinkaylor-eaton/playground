namespace Core;

/// <summary>
/// Adds creation and modification audit tracking to an entity.
/// </summary>
public interface IAuditEntity
{
    /// <summary>
    /// The date and time when the entity was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// The identifier of the user who created the entity.
    /// </summary>
    string? CreatedBy { get; }

    /// <summary>
    /// The date and time when the entity was last modified.
    /// </summary>
    DateTimeOffset? LastModifiedAt { get; }

    /// <summary>
    /// The identifier of the user who last modified the entity.
    /// </summary>
    string? LastModifiedBy { get; }
}