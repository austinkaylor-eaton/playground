using System.Diagnostics;

namespace Core.CQRS;

/// <summary>
/// Provides the shared <see cref="ActivitySource"/> used by CQRS tracing decorators.
/// Consumers must register this source name with their tracing provider (e.g., OpenTelemetry)
/// to capture CQRS activities.
/// </summary>
/// <example>
/// Register the activity source with OpenTelemetry in your host configuration:
/// <code>
/// builder.Services.AddOpenTelemetry()
///     .WithTracing(tracing => tracing
///         .AddSource(CqrsActivitySource.Name));
/// </code>
/// </example>
public static class CqrsActivitySource
{
    /// <summary>
    /// The name of the activity source. Use this value when configuring your tracing provider.
    /// </summary>
    public const string Name = "Core.CQRS";

    /// <summary>
    /// The shared <see cref="ActivitySource"/> instance for CQRS operations.
    /// </summary>
    internal static readonly ActivitySource Source = new(Name);
}