using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Core.CQRS.Decorators.Logging;

/// <summary>
/// Logging decorator for command handlers that return a <typeparamref name="TResponse"/>.
/// Logs before and after command execution, including elapsed time and any exceptions.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
/// <typeparam name="TResponse">The type of the command result.</typeparam>
/// <seealso cref="ICommandHandler{TCommand, TResponse}"/>
public sealed class LoggingCommandHandler<TCommand, TResponse>
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ICommandHandler<TCommand, TResponse> _inner;
    private readonly ILogger<LoggingCommandHandler<TCommand, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="LoggingCommandHandler{TCommand, TResponse}"/> class.
    /// </summary>
    /// <param name="inner">The inner command handler to decorate.</param>
    /// <param name="logger">The logger instance.</param>
    public LoggingCommandHandler(
        ICommandHandler<TCommand, TResponse> inner,
        ILogger<LoggingCommandHandler<TCommand, TResponse>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "S2139",
        Justification = "Decorator intentionally logs and rethrows to preserve the call stack")]
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        Log.HandlingCommand(_logger, commandName);
        var startTime = Stopwatch.GetTimestamp();

        try
        {
            var result = await _inner.Handle(command, cancellationToken).ConfigureAwait(false);

            var elapsed = Stopwatch.GetElapsedTime(startTime);
            Log.HandledCommand(_logger, commandName, elapsed.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            var elapsed = Stopwatch.GetElapsedTime(startTime);
            Log.CommandFailed(_logger, ex, commandName, elapsed.TotalMilliseconds);

            throw;
        }
    }
}


