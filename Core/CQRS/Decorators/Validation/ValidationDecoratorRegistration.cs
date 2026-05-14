using Core.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.CQRS.Decorators.Validation;

/// <summary>
/// Extension methods for registering CQRS handlers with validation decoration.
/// Provides a Scrutor-free alternative for wiring validation decorator chains via the built-in DI container.
/// </summary>
public static class ValidationDecoratorRegistration
{
    /// <summary>
    /// Registers a <see cref="ICommandHandler{TCommand}"/> with validation decoration.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// // Register the validator and handler together:
    /// services.AddValidator&lt;DeleteUserCommand, DeleteUserCommandValidator&gt;();
    /// services.AddCommandHandlerWithValidation&lt;DeleteUserCommand, DeleteUserCommandHandler&gt;();
    ///
    /// // The validator runs before the handler. If validation fails,
    /// // a ValidationException is thrown and the handler is never invoked.
    /// </code>
    /// </example>
    public static IServiceCollection AddCommandHandlerWithValidation<TCommand, THandler>(
        this IServiceCollection services)
        where TCommand : ICommand
        where THandler : class, ICommandHandler<TCommand>
    {
        services.AddScoped<THandler>();
        services.AddScoped<ICommandHandler<TCommand>>(sp =>
            new ValidationCommandHandler<TCommand>(
                sp.GetRequiredService<THandler>(),
                sp.GetRequiredService<IEnumerable<IValidator<TCommand>>>(),
                sp.GetRequiredService<ILogger<ValidationCommandHandler<TCommand>>>()));

        return services;
    }

    /// <summary>
    /// Registers a <see cref="ICommandHandler{TCommand, TResponse}"/> with validation decoration.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// // Register the validator and handler together:
    /// services.AddValidator&lt;CreateUserCommand, CreateUserCommandValidator&gt;();
    /// services.AddCommandHandlerWithValidation&lt;CreateUserCommand, Guid, CreateUserCommandHandler&gt;();
    ///
    /// // Multiple validators can be registered for the same command:
    /// services.AddValidator&lt;CreateUserCommand, CreateUserEmailValidator&gt;();
    /// services.AddValidator&lt;CreateUserCommand, CreateUserNameValidator&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddCommandHandlerWithValidation<TCommand, TResponse, THandler>(
        this IServiceCollection services)
        where TCommand : ICommand<TResponse>
        where THandler : class, ICommandHandler<TCommand, TResponse>
    {
        services.AddScoped<THandler>();
        services.AddScoped<ICommandHandler<TCommand, TResponse>>(sp =>
            new ValidationCommandHandlerWithResponse<TCommand, TResponse>(
                sp.GetRequiredService<THandler>(),
                sp.GetRequiredService<IEnumerable<IValidator<TCommand>>>(),
                sp.GetRequiredService<ILogger<ValidationCommandHandlerWithResponse<TCommand, TResponse>>>()));

        return services;
    }

    /// <summary>
    /// Registers a <see cref="IQueryHandler{TQuery, TResponse}"/> with validation decoration.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// // Register the validator and handler together:
    /// services.AddValidator&lt;GetUserByIdQuery, GetUserByIdQueryValidator&gt;();
    /// services.AddQueryHandlerWithValidation&lt;GetUserByIdQuery, UserResponse, GetUserByIdQueryHandler&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddQueryHandlerWithValidation<TQuery, TResponse, THandler>(
        this IServiceCollection services)
        where TQuery : IQuery<TResponse>
        where THandler : class, IQueryHandler<TQuery, TResponse>
    {
        services.AddScoped<THandler>();
        services.AddScoped<IQueryHandler<TQuery, TResponse>>(sp =>
            new ValidationQueryHandler<TQuery, TResponse>(
                sp.GetRequiredService<THandler>(),
                sp.GetRequiredService<IEnumerable<IValidator<TQuery>>>(),
                sp.GetRequiredService<ILogger<ValidationQueryHandler<TQuery, TResponse>>>()));

        return services;
    }

    /// <summary>
    /// Registers a validator implementation for the specified type.
    /// </summary>
    /// <typeparam name="T">The type being validated (command or query).</typeparam>
    /// <typeparam name="TValidator">The validator implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// // Register a single validator:
    /// services.AddValidator&lt;CreateUserCommand, CreateUserCommandValidator&gt;();
    ///
    /// // Register multiple validators for the same type (all will be executed):
    /// services.AddValidator&lt;CreateUserCommand, CreateUserEmailValidator&gt;();
    /// services.AddValidator&lt;CreateUserCommand, CreateUserNameValidator&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddValidator<T, TValidator>(
        this IServiceCollection services)
        where TValidator : class, IValidator<T>
    {
        services.AddScoped<IValidator<T>, TValidator>();

        return services;
    }
}
