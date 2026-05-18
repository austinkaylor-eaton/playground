using System.Diagnostics;

namespace Core.CQRS.Decorators.Metrics;

/// <summary>
/// A decorator that wraps an <see cref="IQueryHandler{TQuery, TResponse}"/> with
/// metrics instrumentation, recording execution counts, durations, and failure rates
/// via <see cref="System.Diagnostics.Metrics"/> instruments.
/// </summary>
/// <typeparam name="TQuery">The type of query being handled.</typeparam>
/// <typeparam name="TResponse">The type of response produced by the handler.</typeparam>
/// <example>
/// Register with the decorator builder:
/// <code>
/// services.AddQueryHandler&lt;GetUserByIdQuery, UserResponse, GetUserByIdQueryHandler&gt;()
///     .WithMetrics()
///     .Build();
/// </code>
/// </example>
/// <seealso cref="CqrsMetrics"/>
/// <seealso cref="MetricsCommandHandler{TCommand, TResponse}"/>
/// <remarks>
/// Initializes a new instance of the <see cref="MetricsQueryHandler{TQuery, TResponse}"/> class.
/// </remarks>
/// <param name="inner">The inner query handler to decorate.</param>
public sealed class MetricsQueryHandler<TQuery, TResponse>(IQueryHandler<TQuery, TResponse> inner)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
    {
        var queryName = typeof(TQuery).Name;

        var tags = new TagList
        {
            { "cqrs.type", "query" },
            { "cqrs.name", queryName }
        };

        CqrsMetrics.ExecutionCount.Add(1, tags);

        var startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            var result = await inner.Handle(query, cancellationToken).ConfigureAwait(false);

            var elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            CqrsMetrics.Duration.Record(elapsed.TotalMilliseconds, tags);

            return result;
        }
        catch
        {
            var elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            CqrsMetrics.FailureCount.Add(1, tags);
            CqrsMetrics.Duration.Record(elapsed.TotalMilliseconds, tags);

            throw;
        }
    }
}

