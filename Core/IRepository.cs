namespace Core;

/// <summary>
/// Generic repository abstraction for aggregate roots.
/// </summary>
public interface IRepository<T, in TId>
    where T : Entity<TId>
    where TId : notnull
{
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    void Remove(T entity);
}