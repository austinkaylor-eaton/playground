namespace Core;

/// <summary>
/// Generic repository abstraction for aggregate roots.
/// </summary>
/// <typeparam name="T">The entity type, which must derive from <see cref="Entity{TId}"/>.</typeparam>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
/// <remarks>
/// <para>
/// Provides standard CRUD operations for persisting and retrieving domain entities.
/// Implement this interface per aggregate root in your data access layer.
/// </para>
/// </remarks>
/// <example>
/// Define a repository for an Order aggregate:
/// <code>
/// public interface IOrderRepository : IRepository&lt;Order, Guid&gt;
/// {
///     Task&lt;IReadOnlyList&lt;Order&gt;&gt; GetByCustomerAsync(Guid customerId, CancellationToken ct = default);
/// }
/// </code>
/// </example>
/// <seealso cref="Entity{TIdentifier}"/>
public interface IRepository<T, in TId>
    where T : Entity<TId>
    where TId : notnull
{
    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous add operation.</returns>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity with updated state.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity from the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity for removal from the repository during the next unit of work commit.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(T entity);
}