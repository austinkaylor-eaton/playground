namespace Core;

/// <summary>
/// Adds optimistic concurrency support via a row version / concurrency token.
/// </summary>
/// <remarks>
/// <para>
/// Implement this interface on entities that require optimistic concurrency control.
/// The <see cref="RowVersion"/> value is typically managed by the database (e.g., a SQL Server <c>rowversion</c> column)
/// and is checked during updates to detect conflicting modifications.
/// </para>
/// </remarks>
/// <seealso cref="Entity{TIdentifier}"/>
public interface IVersionedEntity
{
    /// <summary>
    /// Gets the concurrency token used for optimistic locking.
    /// </summary>
    /// <value>A byte sequence that changes on every update, used to detect concurrent modifications.</value>
    ReadOnlyMemory<byte> RowVersion { get; }
}