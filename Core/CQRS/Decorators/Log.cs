using Microsoft.Extensions.Logging;

namespace Core.CQRS.Decorators;

/// <summary>
/// High-performance log message definitions for CQRS decorator logging.
/// Uses the <see cref="LoggerMessageAttribute"/> source generator to avoid boxing and allocations.
/// </summary>
internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Handling command {CommandName}")]
    internal static partial void HandlingCommand(ILogger logger, string commandName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Handled command {CommandName} in {ElapsedMs}ms")]
    internal static partial void HandledCommand(ILogger logger, string commandName, double elapsedMs);

    [LoggerMessage(Level = LogLevel.Error, Message = "Command {CommandName} failed after {ElapsedMs}ms")]
    internal static partial void CommandFailed(ILogger logger, Exception ex, string commandName, double elapsedMs);

    [LoggerMessage(Level = LogLevel.Information, Message = "Handling query {QueryName}")]
    internal static partial void HandlingQuery(ILogger logger, string queryName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Handled query {QueryName} in {ElapsedMs}ms")]
    internal static partial void HandledQuery(ILogger logger, string queryName, double elapsedMs);

    [LoggerMessage(Level = LogLevel.Error, Message = "Query {QueryName} failed after {ElapsedMs}ms")]
    internal static partial void QueryFailed(ILogger logger, Exception ex, string queryName, double elapsedMs);
}

