namespace Core;

/// <summary>
/// Interface for entities that support soft deletion, allowing them to be marked as deleted without being physically removed from the database.
/// </summary>
/// <remarks>
/// <para>
/// Soft-deleted entities remain in the data store but are excluded from standard queries.
/// Use <see cref="Restore"/> to reverse a soft deletion.
/// </para>
/// </remarks>
/// <example>
/// Implement soft deletion on a domain entity:
/// <code>
/// public class Customer : Entity&lt;Guid&gt;, ISoftDeleteEntity
/// {
///     public bool IsDeleted { get; private set; }
///     public DateTimeOffset? DeletedAt { get; private set; }
///     public string? DeletedBy { get; private set; }
///
///     public void SoftDelete(string? deletedBy = null)
///     {
///         IsDeleted = true;
///         DeletedAt = DateTimeOffset.UtcNow;
///         DeletedBy = deletedBy;
///     }
///
///     public void Restore()
///     {
///         IsDeleted = false;
///         DeletedAt = null;
///         DeletedBy = null;
///     }
/// }
/// </code>
/// </example>
/// <seealso cref="IActivateEntity"/>
public interface ISoftDeleteEntity
{
    /// <summary>
    /// Gets a value indicating whether the entity has been marked as deleted.
    /// </summary>
    /// <value><c>true</c> if the entity is soft-deleted; otherwise, <c>false</c>.</value>
    bool IsDeleted { get; }

    /// <summary>
    /// Gets the date and time when the entity was marked as deleted.
    /// </summary>
    /// <value>The <see cref="DateTimeOffset"/> of the deletion, or <c>null</c> if the entity is not deleted.</value>
    DateTimeOffset? DeletedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who marked the entity as deleted.
    /// </summary>
    /// <value>The user identifier, or <c>null</c> if not specified or the entity is not deleted.</value>
    string? DeletedBy { get; }

    /// <summary>
    /// Marks the entity as deleted.
    /// </summary>
    /// <param name="deletedBy">The optional identifier of the user performing the deletion.</param>
    void SoftDelete(string? deletedBy = null);

    /// <summary>
    /// Restores a soft-deleted entity, clearing the deletion metadata.
    /// </summary>
    void Restore();
}