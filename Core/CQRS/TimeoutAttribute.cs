namespace Core.CQRS;

/// <summary>
/// Specifies a timeout duration for a CQRS command or query handler.
/// When applied to a command or query type, the <see cref="TimeoutCommandHandler{TCommand}"/>
/// and related decorators will cancel execution if it exceeds the specified duration.
/// </summary>
/// <example>
/// <code>
/// [Timeout(timeoutMs: 5000)]
/// public sealed record GenerateReportCommand(Guid ReportId) : ICommand;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class TimeoutAttribute : Attribute
{
    public int TimeoutMs { get; }

    /// <summary>
    /// Gets the timeout duration.
    /// </summary>
    public TimeSpan Duration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeoutAttribute"/> class.
    /// </summary>
    /// <param name="timeoutMs">The timeout in milliseconds.</param>
    /// <example>
    /// <code>
    /// [Timeout(timeoutMs: 3000)]
    /// public sealed record SlowCommand : ICommand;
    /// </code>
    /// </example>
    public TimeoutAttribute(int timeoutMs)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeoutMs);
        TimeoutMs = timeoutMs;
        Duration = TimeSpan.FromMilliseconds(timeoutMs);
    }
}