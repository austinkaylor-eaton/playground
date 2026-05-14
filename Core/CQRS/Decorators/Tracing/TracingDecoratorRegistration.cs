using Microsoft.Extensions.DependencyInjection;

namespace Core.CQRS.Decorators.Tracing;

/// <summary>
/// Extension methods for registering CQRS handlers with activity tracing decoration.
/// Uses <see cref="System.Diagnostics.ActivitySource"/> from the BCL — no external dependencies required.
/// </summary>
public static class TracingDecoratorRegistration
{
    /// <summary>
    /// Registers a <see cref="ICommandHandler{TCommand}"/> with activity tracing decoration.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// // Register the handler with tracing:
    /// services.AddCommandHandlerWithTracing&lt;DeleteUserCommand, DeleteUserCommandHandler&gt;();
    ///
    /// // Then register the activity source with your tracing provider:
    /// builder.Services.AddOpenTelemetry()
    ///     .WithTracing(tracing => tracing
    ///         .AddSource(CqrsActivitySource.Name));
    /// </code>
    /// </example>
    public static IServiceCollection AddCommandHandlerWithTracing<TCommand, THandler>(
        this IServiceCollection services)
        where TCommand : ICommand
        where THandler : class, ICommandHandler<TCommand>
    {
        services.AddScoped<THandler>();
        services.AddScoped<ICommandHandler<TCommand>>(sp =>
            new TracingCommandHandler<TCommand>(
                sp.GetRequiredService<THandler>()));

        return services;
    }

    /// <summary>
    /// Registers a <see cref="ICommandHandler{TCommand, TResponse}"/> with activity tracing decoration.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// // Register the handler with tracing:
    /// services.AddCommandHandlerWithTracing&lt;CreateUserCommand, Guid, CreateUserCommandHandler&gt;();
    ///
    /// // Then register the activity source with your tracing provider:
    /// builder.Services.AddOpenTelemetry()
    ///     .WithTracing(tracing => tracing
    ///         .AddSource(CqrsActivitySource.Name));
    /// </code>
    /// </example>
    public static IServiceCollection AddCommandHandlerWithTracing<TCommand, TResponse, THandler>(
        this IServiceCollection services)
        where TCommand : ICommand<TResponse>
        where THandler : class, ICommandHandler<TCommand, TResponse>
    {
        services.AddScoped<THandler>();
        services.AddScoped<ICommandHandler<TCommand, TResponse>>(sp =>
            new TracingCommandHandlerWithResponse<TCommand, TResponse>(
                sp.GetRequiredService<THandler>()));

        return services;
    }

    /// <summary>
    /// Registers a <see cref="IQueryHandler{TQuery, TResponse}"/> with activity tracing decoration.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// // Register the handler with tracing:
    /// services.AddQueryHandlerWithTracing&lt;GetUserByIdQuery, UserResponse, GetUserByIdQueryHandler&gt;();
    ///
    /// // Then register the activity source with your tracing provider:
    /// builder.Services.AddOpenTelemetry()
    ///     .WithTracing(tracing => tracing
    ///         .AddSource(CqrsActivitySource.Name));
    /// </code>
    /// </example>
    public static IServiceCollection AddQueryHandlerWithTracing<TQuery, TResponse, THandler>(
        this IServiceCollection services)
        where TQuery : IQuery<TResponse>
        where THandler : class, IQueryHandler<TQuery, TResponse>
    {
        services.AddScoped<THandler>();
        services.AddScoped<IQueryHandler<TQuery, TResponse>>(sp =>
            new TracingQueryHandler<TQuery, TResponse>(
                sp.GetRequiredService<THandler>()));

        return services;
    }
}