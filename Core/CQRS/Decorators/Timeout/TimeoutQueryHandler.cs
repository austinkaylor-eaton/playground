namespace Core.CQRS.Decorators.Timeout;

/// <summary>
/// Timeout decorator for query handlers.
/// If the query type is annotated with <see cref="TimeoutAttribute"/>, execution is
/// cancelled after the specified duration and a <see cref="TimeoutException"/> is thrown.
/// If no attribute is present, the decorator is a no-op pass-through.
/// </summary>
/// <typeparam name="TQuery">The type of query being handled.</typeparam>
/// <typeparam name="TResponse">The type of the query result.</typeparam>
/// <example>
/// <code>
/// [Timeout(timeoutMs: 2000)]
/// public sealed record GetLargeReportQuery(Guid ReportId) : IQuery&lt;ReportDto&gt;;
///
/// services.AddQueryHandlerWithTimeout&lt;GetLargeReportQuery, ReportDto, GetLargeReportQueryHandler&gt;();
/// </code>
/// </example>
/// <seealso cref="IQueryHandler{TQuery, TResponse}"/>
/// <seealso cref="TimeoutAttribute"/>
/// <remarks>
/// Initializes a new instance of the <see cref="TimeoutQueryHandler{TQuery, TResponse}"/> class.
/// </remarks>
/// <param name="inner">The inner query handler to decorate.</param>
public sealed class TimeoutQueryHandler<TQuery, TResponse>(IQueryHandler<TQuery, TResponse> inner) : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private static readonly TimeSpan? ConfiguredTimeout =
        typeof(TQuery).GetCustomAttributes(typeof(TimeoutAttribute), false) is [TimeoutAttribute attr, ..]
            ? attr.Duration
            : null;

    /// <inheritdoc />
    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
    {
        if (ConfiguredTimeout is not { } timeout)
        {
            return await inner.Handle(query, cancellationToken).ConfigureAwait(false);
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        try
        {
            return await inner.Handle(query, cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"Query {typeof(TQuery).Name} exceeded the configured timeout of {timeout.TotalMilliseconds}ms.");
        }
    }
}