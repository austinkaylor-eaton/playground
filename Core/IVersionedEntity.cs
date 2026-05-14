namespace Core;

/// <summary>
/// Adds optimistic concurrency support via a row version / concurrency token.
/// </summary>
public interface IVersionedEntity
{
    /// <summary>
    /// The concurrency token used for optimistic locking.
    /// </summary>
    byte[] RowVersion { get; }
}