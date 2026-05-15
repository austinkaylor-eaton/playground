using Microsoft.Extensions.DependencyInjection;

namespace Core.CQRS.Decorators.Timeout;

/// <summary>
/// Extension methods for registering CQRS handlers with timeout decoration.
/// Timeout duration is configured via <see cref="TimeoutAttribute"/> on the command or query type.
/// </summary>
public static class TimeoutDecoratorRegistration
{
    /// <summary>
    /// Registers a <see cref="ICommandHandler{TCommand, TResponse}"/> with timeout decoration.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// [Timeout(timeoutMs: 3000)]
    /// public sealed record CreateOrderCommand(string Product) : ICommand&lt;Guid&gt;;
    ///
    /// services.AddCommandHandlerWithTimeout&lt;CreateOrderCommand, Guid, CreateOrderCommandHandler&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddCommandHandlerWithTimeout<TCommand, TResponse, THandler>(
        this IServiceCollection services)
        where TCommand : ICommand<TResponse>
        where THandler : class, ICommandHandler<TCommand, TResponse>
    {
        services.AddScoped<THandler>();
        services.AddScoped<ICommandHandler<TCommand, TResponse>>(sp =>
            new TimeoutCommandHandler<TCommand, TResponse>(
                sp.GetRequiredService<THandler>()));

        return services;
    }

    /// <summary>
    /// Registers a <see cref="IQueryHandler{TQuery, TResponse}"/> with timeout decoration.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// [Timeout(timeoutMs: 2000)]
    /// public sealed record GetLargeReportQuery(Guid ReportId) : IQuery&lt;ReportDto&gt;;
    ///
    /// services.AddQueryHandlerWithTimeout&lt;GetLargeReportQuery, ReportDto, GetLargeReportQueryHandler&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddQueryHandlerWithTimeout<TQuery, TResponse, THandler>(
        this IServiceCollection services)
        where TQuery : IQuery<TResponse>
        where THandler : class, IQueryHandler<TQuery, TResponse>
    {
        services.AddScoped<THandler>();
        services.AddScoped<IQueryHandler<TQuery, TResponse>>(sp =>
            new TimeoutQueryHandler<TQuery, TResponse>(
                sp.GetRequiredService<THandler>()));

        return services;
    }
}