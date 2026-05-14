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
public sealed partial class LoggingCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> inner;
    private readonly ILogger<LoggingCommandHandler<TCommand>> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingCommandHandler{TCommand}"/> class.
    /// </summary>
    /// <param name="inner">The inner command handler to decorate.</param>
    /// <param name="logger">The logger instance.</param>
    public LoggingCommandHandler(
        ICommandHandler<TCommand> inner,
        ILogger<LoggingCommandHandler<TCommand>> logger)
    {
        this.inner = inner;
        this.logger = logger;
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "S2139",
        Justification = "Decorator intentionally logs and rethrows to preserve the call stack")]
    public async Task Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        LogHandlingCommand(logger, commandName);
        var startTime = Stopwatch.GetTimestamp();

        try
        {
            await inner.Handle(command, cancellationToken).ConfigureAwait(false);

            var elapsed = Stopwatch.GetElapsedTime(startTime);
            LogHandledCommand(logger, commandName, elapsed.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            var elapsed = Stopwatch.GetElapsedTime(startTime);
            LogCommandFailed(logger, ex, commandName, elapsed.TotalMilliseconds);

            throw;
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Handling command {CommandName}")]
    private static partial void LogHandlingCommand(ILogger logger, string commandName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Handled command {CommandName} in {ElapsedMs}ms")]
    private static partial void LogHandledCommand(ILogger logger, string commandName, double elapsedMs);

    [LoggerMessage(Level = LogLevel.Error, Message = "Command {CommandName} failed after {ElapsedMs}ms")]
    private static partial void LogCommandFailed(ILogger logger, Exception ex, string commandName, double elapsedMs);
}


