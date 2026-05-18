using Core.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Core.CQRS.Decorators;

/// <summary>
/// Extension methods for registering CQRS handlers with a fluent decorator builder.
/// Replaces the individual per-decorator registration classes with a single composable API.
/// </summary>
/// <example>
/// <code>
/// services.AddCommandHandler&lt;CreateUserCommand, Guid, CreateUserCommandHandler&gt;()
///     .WithValidation()
///     .WithLogging()
///     .WithTracing()
///     .WithTimeout()
///     .Build();
///
/// services.AddQueryHandler&lt;GetUserByIdQuery, UserResponse, GetUserByIdQueryHandler&gt;()
///     .WithLogging()
///     .WithTracing()
///     .Build();
/// </code>
/// </example>
public static class HandlerRegistrationExtensions
{
    /// <summary>
    /// Begins a fluent registration for a command handler, returning a builder
    /// that allows decorators to be composed via <c>.With*()</c> calls.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>A <see cref="CommandHandlerBuilder{TCommand, TResponse, THandler}"/> for fluent configuration.</returns>
    /// <example>
    /// <code>
    /// services.AddCommandHandler&lt;CreateUserCommand, Guid, CreateUserCommandHandler&gt;()
    ///     .WithValidation()
    ///     .WithLogging()
    ///     .Build();
    /// </code>
    /// </example>
    public static CommandHandlerBuilder<TCommand, TResponse, THandler> AddCommandHandler<TCommand, TResponse, THandler>(
        this IServiceCollection services)
        where TCommand : ICommand<TResponse>
        where THandler : class, ICommandHandler<TCommand, TResponse>
    {
        return new CommandHandlerBuilder<TCommand, TResponse, THandler>(services);
    }

    /// <summary>
    /// Begins a fluent registration for a query handler, returning a builder
    /// that allows decorators to be composed via <c>.With*()</c> calls.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="THandler">The concrete handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>A <see cref="QueryHandlerBuilder{TQuery, TResponse, THandler}"/> for fluent configuration.</returns>
    /// <example>
    /// <code>
    /// services.AddQueryHandler&lt;GetUserByIdQuery, UserResponse, GetUserByIdQueryHandler&gt;()
    ///     .WithLogging()
    ///     .WithTracing()
    ///     .Build();
    /// </code>
    /// </example>
    public static QueryHandlerBuilder<TQuery, TResponse, THandler> AddQueryHandler<TQuery, TResponse, THandler>(
        this IServiceCollection services)
        where TQuery : IQuery<TResponse>
        where THandler : class, IQueryHandler<TQuery, TResponse>
    {
        return new QueryHandlerBuilder<TQuery, TResponse, THandler>(services);
    }

    /// <summary>
    /// Registers a validator implementation for the specified type.
    /// Validators are consumed by the validation decorator when <c>.WithValidation()</c> is used.
    /// </summary>
    /// <typeparam name="T">The type being validated (command or query).</typeparam>
    /// <typeparam name="TValidator">The validator implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddValidator&lt;CreateUserCommand, CreateUserCommandValidator&gt;();
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

