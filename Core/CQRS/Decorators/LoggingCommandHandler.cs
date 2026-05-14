using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Core.CQRS.Decorators;

/// <summary>
/// Logging decorator for command handlers that do not return a value.
/// Logs before and after command execution, including elapsed time and any exceptions.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
/// <seealso cref="ICommandHandler{TCommand}"/>
public sealed class LoggingCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _inner;
    private readonly ILogger<LoggingCommandHandler<TCommand>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingCommandHandler{TCommand}"/> class.
    /// </summary>
    /// <param name="inner">The inner command handler to decorate.</param>
    /// <param name="logger">The logger instance.</param>
    public LoggingCommandHandler(
        ICommandHandler<TCommand> inner,
        ILogger<LoggingCommandHandler<TCommand>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "S2139",
        Justification = "Decorator intentionally logs and rethrows to preserve the call stack")]
    public async Task Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        Log.HandlingCommand(_logger, commandName);
        var startTime = Stopwatch.GetTimestamp();

        try
        {
            await _inner.Handle(command, cancellationToken).ConfigureAwait(false);

            var elapsed = Stopwatch.GetElapsedTime(startTime);
            Log.HandledCommand(_logger, commandName, elapsed.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            var elapsed = Stopwatch.GetElapsedTime(startTime);
            Log.CommandFailed(_logger, ex, commandName, elapsed.TotalMilliseconds);

            throw;
        }
    }
}


