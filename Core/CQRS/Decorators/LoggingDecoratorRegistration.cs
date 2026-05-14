using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.CQRS.Decorators;

/// <summary>
/// Extension methods for registering CQRS handlers with logging decoration.
/// Provides a Scrutor-free alternative for wiring decorator chains via the built-in DI container.
/// </summary>
public static class LoggingDecoratorRegistration
{
    /// <summary>
    /// Registers a <see cref="ICommandHandler{TCommand}"/> with logging decoration.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddCommandHandlerWithLogging&lt;DeleteUserCommand, DeleteUserCommandHandler&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddCommandHandlerWithLogging<TCommand, THandler>(
        this IServiceCollection services)
        where TCommand : ICommand
        where THandler : class, ICommandHandler<TCommand>
    {
        services.AddScoped<THandler>();
        services.AddScoped<ICommandHandler<TCommand>>(sp =>
            new LoggingCommandHandler<TCommand>(
                sp.GetRequiredService<THandler>(),
                sp.GetRequiredService<ILogger<LoggingCommandHandler<TCommand>>>()));

        return services;
    }

    /// <summary>
    /// Registers a <see cref="ICommandHandler{TCommand, TResponse}"/> with logging decoration.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddCommandHandlerWithLogging&lt;CreateUserCommand, Guid, CreateUserCommandHandler&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddCommandHandlerWithLogging<TCommand, TResponse, THandler>(
        this IServiceCollection services)
        where TCommand : ICommand<TResponse>
        where THandler : class, ICommandHandler<TCommand, TResponse>
    {
        services.AddScoped<THandler>();
        services.AddScoped<ICommandHandler<TCommand, TResponse>>(sp =>
            new LoggingCommandHandlerWithResponse<TCommand, TResponse>(
                sp.GetRequiredService<THandler>(),
                sp.GetRequiredService<ILogger<LoggingCommandHandlerWithResponse<TCommand, TResponse>>>()));

        return services;
    }

    /// <summary>
    /// Registers a <see cref="IQueryHandler{TQuery, TResponse}"/> with logging decoration.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddQueryHandlerWithLogging&lt;GetUserByIdQuery, UserResponse, GetUserByIdQueryHandler&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddQueryHandlerWithLogging<TQuery, TResponse, THandler>(
        this IServiceCollection services)
        where TQuery : IQuery<TResponse>
        where THandler : class, IQueryHandler<TQuery, TResponse>
    {
        services.AddScoped<THandler>();
        services.AddScoped<IQueryHandler<TQuery, TResponse>>(sp =>
            new LoggingQueryHandler<TQuery, TResponse>(
                sp.GetRequiredService<THandler>(),
                sp.GetRequiredService<ILogger<LoggingQueryHandler<TQuery, TResponse>>>()));

        return services;
    }
}

