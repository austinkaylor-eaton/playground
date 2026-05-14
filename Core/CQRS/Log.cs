using Microsoft.Extensions.Logging;

namespace Core.CQRS;

/// <summary>
/// High-performance log message definitions for CQRS decorator logging.
/// Uses the <see cref="LoggerMessageAttribute"/> source generator to avoid boxing and allocations.
/// </summary>
internal static partial class Log
{
    /// <summary>
    /// Logs that a command is about to be handled.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="commandName">The name of the command type.</param>
    [LoggerMessage(Level = LogLevel.Information, Message = "Handling command {CommandName}")]
    internal static partial void HandlingCommand(ILogger logger, string commandName);

    /// <summary>
    /// Logs that a command was successfully handled, including elapsed time.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="commandName">The name of the command type.</param>
    /// <param name="elapsedMs">The elapsed time in milliseconds.</param>
    [LoggerMessage(Level = LogLevel.Information, Message = "Handled command {CommandName} in {ElapsedMs}ms")]
    internal static partial void HandledCommand(ILogger logger, string commandName, double elapsedMs);

    /// <summary>
    /// Logs that a command failed with an exception, including elapsed time.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="ex">The exception that caused the failure.</param>
    /// <param name="commandName">The name of the command type.</param>
    /// <param name="elapsedMs">The elapsed time in milliseconds before the failure.</param>
    [LoggerMessage(Level = LogLevel.Error, Message = "Command {CommandName} failed after {ElapsedMs}ms")]
    internal static partial void CommandFailed(ILogger logger, Exception ex, string commandName, double elapsedMs);

    /// <summary>
    /// Logs that a query is about to be handled.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="queryName">The name of the query type.</param>
    [LoggerMessage(Level = LogLevel.Information, Message = "Handling query {QueryName}")]
    internal static partial void HandlingQuery(ILogger logger, string queryName);

    /// <summary>
    /// Logs that a query was successfully handled, including elapsed time.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="queryName">The name of the query type.</param>
    /// <param name="elapsedMs">The elapsed time in milliseconds.</param>
    [LoggerMessage(Level = LogLevel.Information, Message = "Handled query {QueryName} in {ElapsedMs}ms")]
    internal static partial void HandledQuery(ILogger logger, string queryName, double elapsedMs);

    /// <summary>
    /// Logs that a query failed with an exception, including elapsed time.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="ex">The exception that caused the failure.</param>
    /// <param name="queryName">The name of the query type.</param>
    /// <param name="elapsedMs">The elapsed time in milliseconds before the failure.</param>
    [LoggerMessage(Level = LogLevel.Error, Message = "Query {QueryName} failed after {ElapsedMs}ms")]
    internal static partial void QueryFailed(ILogger logger, Exception ex, string queryName, double elapsedMs);

    /// <summary>
    /// Logs that validation failed for a command or query, including the number of errors.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="name">The name of the command or query type.</param>
    /// <param name="errorCount">The number of validation errors.</param>
    [LoggerMessage(Level = LogLevel.Warning, Message = "Validation failed for {Name} with {ErrorCount} error(s)")]
    internal static partial void ValidationFailed(ILogger logger, string name, int errorCount);
}
