using Core.CQRS.Decorators.Logging;
using Core.CQRS.Decorators.Timeout;
using Core.CQRS.Decorators.Tracing;
using Core.CQRS.Decorators.Validation;
using Core.CQRS.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.CQRS.Decorators;

/// <summary>
/// Fluent builder that composes a decorator chain around a command handler registration.
/// Decorators are applied inside-out: the last <c>.With*()</c> call becomes the outermost decorator.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <typeparam name="THandler">The concrete handler type.</typeparam>
/// <example>
/// <code>
/// services.AddCommandHandler&lt;CreateUserCommand, Guid, CreateUserCommandHandler&gt;()
///     .WithValidation()
///     .WithLogging()
///     .WithTracing()
///     .WithTimeout()
///     .Build();
/// </code>
/// </example>
public sealed class CommandHandlerBuilder<TCommand, TResponse, THandler>
    where TCommand : ICommand<TResponse>
    where THandler : class, ICommandHandler<TCommand, TResponse>
{
    private readonly IServiceCollection _services;
    private readonly List<Func<IServiceProvider, ICommandHandler<TCommand, TResponse>, ICommandHandler<TCommand, TResponse>>> _decoratorFactories = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandHandlerBuilder{TCommand, TResponse, THandler}"/> class.
    /// </summary>
    /// <param name="services">The service collection to register the handler into.</param>
    internal CommandHandlerBuilder(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// Adds the logging decorator to the command handler chain.
    /// Logs before and after execution, including elapsed time and exceptions.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public CommandHandlerBuilder<TCommand, TResponse, THandler> WithLogging()
    {
        _decoratorFactories.Add((sp, inner) =>
            new LoggingCommandHandler<TCommand, TResponse>(
                inner,
                sp.GetRequiredService<ILogger<LoggingCommandHandler<TCommand, TResponse>>>()));

        return this;
    }

    /// <summary>
    /// Adds the validation decorator to the command handler chain.
    /// Runs all registered <see cref="IValidator{T}"/> instances before the inner handler.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public CommandHandlerBuilder<TCommand, TResponse, THandler> WithValidation()
    {
        _decoratorFactories.Add((sp, inner) =>
            new ValidationCommandHandler<TCommand, TResponse>(
                inner,
                sp.GetRequiredService<IEnumerable<IValidator<TCommand>>>(),
                sp.GetRequiredService<ILogger<ValidationCommandHandler<TCommand, TResponse>>>()));

        return this;
    }

    /// <summary>
    /// Adds the tracing decorator to the command handler chain.
    /// Creates an <see cref="System.Diagnostics.Activity"/> span around the handler execution.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public CommandHandlerBuilder<TCommand, TResponse, THandler> WithTracing()
    {
        _decoratorFactories.Add((_, inner) =>
            new TracingCommandHandler<TCommand, TResponse>(inner));

        return this;
    }

    /// <summary>
    /// Adds the timeout decorator to the command handler chain.
    /// Enforces the timeout configured via <see cref="TimeoutAttribute"/> on the command type.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public CommandHandlerBuilder<TCommand, TResponse, THandler> WithTimeout()
    {
        _decoratorFactories.Add((_, inner) =>
            new TimeoutCommandHandler<TCommand, TResponse>(inner));

        return this;
    }

    /// <summary>
    /// Finalizes the builder and registers the decorated command handler in the DI container.
    /// Decorators wrap inside-out: the last <c>.With*()</c> call is the outermost layer.
    /// </summary>
    /// <returns>The service collection for further registrations.</returns>
    public IServiceCollection Build()
    {
        _services.AddScoped<THandler>();

        if (_decoratorFactories.Count is 0)
        {
            _services.AddScoped<ICommandHandler<TCommand, TResponse>>(sp =>
                sp.GetRequiredService<THandler>());

            return _services;
        }

        _services.AddScoped<ICommandHandler<TCommand, TResponse>>(sp =>
        {
            ICommandHandler<TCommand, TResponse> handler = sp.GetRequiredService<THandler>();

            foreach (var factory in _decoratorFactories)
            {
                handler = factory(sp, handler);
            }

            return handler;
        });

        return _services;
    }
}

