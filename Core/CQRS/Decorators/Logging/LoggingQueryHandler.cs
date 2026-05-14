using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Core.CQRS.Decorators.Logging;

/// <summary>
/// Logging decorator for query handlers.
/// Logs before and after query execution, including elapsed time and any exceptions.
/// </summary>
/// <typeparam name="TQuery">The type of query being handled.</typeparam>
/// <typeparam name="TResponse">The type of the query result.</typeparam>
/// <seealso cref="IQueryHandler{TQuery, TResponse}"/>
public sealed class LoggingQueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> _inner;
    private readonly ILogger<LoggingQueryHandler<TQuery, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingQueryHandler{TQuery, TResponse}"/> class.
    /// </summary>
    /// <param name="inner">The inner query handler to decorate.</param>
    /// <param name="logger">The logger instance.</param>
    public LoggingQueryHandler(
        IQueryHandler<TQuery, TResponse> inner,
        ILogger<LoggingQueryHandler<TQuery, TResponse>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "S2139",
        Justification = "Decorator intentionally logs and rethrows to preserve the call stack")]
    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
    {
        var queryName = typeof(TQuery).Name;

        Log.HandlingQuery(_logger, queryName);
        var startTime = Stopwatch.GetTimestamp();

        try
        {
            var result = await _inner.Handle(query, cancellationToken).ConfigureAwait(false);

            var elapsed = Stopwatch.GetElapsedTime(startTime);
            Log.HandledQuery(_logger, queryName, elapsed.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            var elapsed = Stopwatch.GetElapsedTime(startTime);
            Log.QueryFailed(_logger, ex, queryName, elapsed.TotalMilliseconds);

            throw;
        }
    }
}


