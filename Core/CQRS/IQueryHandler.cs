namespace Core.CQRS;

/// <summary>
/// Defines a handler for a query that returns a <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TQuery">The type of the query to handle.</typeparam>
/// <typeparam name="TResponse">The type of the result returned by the query.</typeparam>
/// <example>
/// <code>
/// public sealed record GetUserByIdQuery(Guid UserId) : IQuery&lt;UserResponse&gt;;
///
/// public sealed class GetUserByIdQueryHandler : IQueryHandler&lt;GetUserByIdQuery, UserResponse&gt;
/// {
///     public async Task&lt;UserResponse&gt; Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
///     {
///         // Retrieve user from data store
///     }
/// }
/// </code>
/// </example>
/// <seealso cref="IQuery{TResponse}"/>
/// <seealso href="https://www.milanjovanovic.tech/blog/cqrs-pattern-the-way-it-should-have-been-from-the-start">CQRS Pattern - Milan Jovanović</seealso>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Handles the specified query and returns a result.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the query result.</returns>
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}