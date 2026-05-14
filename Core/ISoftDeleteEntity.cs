namespace Core;

/// <summary>
/// Interface for entities that support soft deletion, allowing them to be marked as deleted without being physically removed from the database.
/// </summary>
public interface ISoftDeleteEntity
{
    /// <summary>
    /// Indicates whether the entity has been marked as deleted.
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    /// The date and time when the entity was marked as deleted.
    /// </summary>
    DateTimeOffset? DeletedAt { get; }

    /// <summary>
    /// The identifier of the user who marked the entity as deleted.
    /// </summary>
    string? DeletedBy { get; }

    /// <summary>
    /// Marks the entity as deleted.
    /// </summary>
    void SoftDelete(string? deletedBy = null);

    /// <summary>
    /// Restores a soft-deleted entity.
    /// </summary>
    void Restore();
}