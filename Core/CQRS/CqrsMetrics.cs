using System.Diagnostics.Metrics;

namespace Core.CQRS;

/// <summary>
/// Provides shared <see cref="Meter"/> and instruments for the Core.CQRS library, enabling
/// metrics collection for command and query handler executions.
/// </summary>
/// <example>
/// Register the meter with OpenTelemetry:
/// <code>
/// builder.Services.AddOpenTelemetry()
///     .WithMetrics(metrics => metrics
///         .AddMeter(CqrsMetrics.MeterName));
/// </code>
/// </example>
public static class CqrsMetrics
{
    /// <summary>
    /// The name of the meter used for CQRS metrics.
    /// Use this constant when registering the meter with your telemetry provider.
    /// </summary>
    public const string MeterName = "Core.CQRS";

    /// <summary>
    /// Gets the shared <see cref="Meter"/> instance used by CQRS metrics decorators.
    /// </summary>
    internal static readonly Meter Meter = new(MeterName);

    /// <summary>
    /// Gets the counter that tracks the total number of handler executions.
    /// </summary>
    internal static readonly Counter<long> ExecutionCount =
        Meter.CreateCounter<long>("cqrs.handler.executions", description: "Total number of CQRS handler executions");

    /// <summary>
    /// Gets the counter that tracks the total number of handler failures.
    /// </summary>
    internal static readonly Counter<long> FailureCount =
        Meter.CreateCounter<long>("cqrs.handler.failures", description: "Total number of CQRS handler failures");

    /// <summary>
    /// Gets the histogram that records handler execution durations in milliseconds.
    /// </summary>
    internal static readonly Histogram<double> Duration =
        Meter.CreateHistogram<double>("cqrs.handler.duration", unit: "ms", description: "Duration of CQRS handler executions");
}

