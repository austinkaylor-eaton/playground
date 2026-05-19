using System.Diagnostics;

namespace Core.CQRS.Decorators.Tracing;

/// <summary>
/// Tracing decorator for query handlers.
/// Starts an <see cref="Activity"/> span around the inner handler execution,
/// recording the query name, type, and success/failure status.
/// </summary>
/// <typeparam name="TQuery">The type of query being handled.</typeparam>
/// <typeparam name="TResponse">The type of the query result.</typeparam>
/// <example>
/// Register the handler with tracing in the DI container:
/// <code>
/// services.AddQueryHandlerWithTracing&lt;GetUserByIdQuery, UserResponse, GetUserByIdQueryHandler&gt;();
/// </code>
/// </example>
/// <seealso cref="IQueryHandler{TQuery, TResponse}"/>
/// <seealso cref="CqrsActivitySource"/>
/// <remarks>
/// Initializes a new instance of the <see cref="TracingQueryHandler{TQuery, TResponse}"/> class.
/// </remarks>
/// <param name="inner">The inner query handler to decorate.</param>
public sealed class TracingQueryHandler<TQuery, TResponse>(IQueryHandler<TQuery, TResponse> inner) :
    IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
    {
        var queryName = typeof(TQuery).Name;

        using var activity = CqrsActivitySource.Source.StartActivity(
            queryName);

        activity?.SetTag("cqrs.type", "query");
        activity?.SetTag("cqrs.name", queryName);

        try
        {
            var result = await inner.Handle(query, cancellationToken).ConfigureAwait(false);

            activity?.SetStatus(ActivityStatusCode.Ok);

            return result;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent("exception",
                tags: new ActivityTagsCollection
                {
                    { "exception.type", ex.GetType().FullName },
                    { "exception.message", ex.Message },
                }));

            throw;
        }
    }
}