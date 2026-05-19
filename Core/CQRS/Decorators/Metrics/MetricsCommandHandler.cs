using System.Diagnostics;

namespace Core.CQRS.Decorators.Metrics;

/// <summary>
/// A decorator that wraps an <see cref="ICommandHandler{TCommand, TResponse}"/> with
/// metrics instrumentation, recording execution counts, durations, and failure rates
/// via <see cref="System.Diagnostics.Metrics"/> instruments.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
/// <typeparam name="TResponse">The type of response produced by the handler.</typeparam>
/// <example>
/// Register with the decorator builder:
/// <code>
/// services.AddCommandHandler&lt;CreateUserCommand, Guid, CreateUserCommandHandler&gt;()
///     .WithMetrics()
///     .Build();
/// </code>
/// </example>
/// <seealso cref="CqrsMetrics"/>
/// <seealso cref="MetricsQueryHandler{TQuery, TResponse}"/>
/// <remarks>
/// Initializes a new instance of the <see cref="MetricsCommandHandler{TCommand, TResponse}"/> class.
/// </remarks>
/// <param name="inner">The inner command handler to decorate.</param>
public sealed class MetricsCommandHandler<TCommand, TResponse>(ICommandHandler<TCommand, TResponse> inner)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        var tags = new TagList
        {
            { "cqrs.type", "command" },
            { "cqrs.name", commandName }
        };

        CqrsMetrics.ExecutionCount.Add(1, tags);

        var startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            var result = await inner.Handle(command, cancellationToken).ConfigureAwait(false);

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

