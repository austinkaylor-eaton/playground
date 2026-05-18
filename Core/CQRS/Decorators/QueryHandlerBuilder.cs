using Core.CQRS.Decorators.Logging;
using Core.CQRS.Decorators.Timeout;
using Core.CQRS.Decorators.Tracing;
using Core.CQRS.Decorators.Validation;
using Core.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.CQRS.Decorators;

/// <summary>
/// Fluent builder that composes a decorator chain around a query handler registration.
/// Decorators are applied inside-out: the last <c>.With*()</c> call becomes the outermost decorator.
/// </summary>
/// <typeparam name="TQuery">The query type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <typeparam name="THandler">The concrete handler type.</typeparam>
/// <example>
/// <code>
/// services.AddQueryHandler&lt;GetUserByIdQuery, UserResponse, GetUserByIdQueryHandler&gt;()
///     .WithValidation()
///     .WithLogging()
///     .WithTracing()
///     .WithTimeout()
///     .Build();
/// </code>
/// </example>
public sealed class QueryHandlerBuilder<TQuery, TResponse, THandler>
    where TQuery : IQuery<TResponse>
    where THandler : class, IQueryHandler<TQuery, TResponse>
{
    private readonly IServiceCollection _services;
    private readonly List<Func<IServiceProvider, IQueryHandler<TQuery, TResponse>, IQueryHandler<TQuery, TResponse>>> _decoratorFactories = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryHandlerBuilder{TQuery, TResponse, THandler}"/> class.
    /// </summary>
    /// <param name="services">The service collection to register the handler into.</param>
    internal QueryHandlerBuilder(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// Adds the logging decorator to the query handler chain.
    /// Logs before and after execution, including elapsed time and exceptions.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public QueryHandlerBuilder<TQuery, TResponse, THandler> WithLogging()
    {
        _decoratorFactories.Add((sp, inner) =>
            new LoggingQueryHandler<TQuery, TResponse>(
                inner,
                sp.GetRequiredService<ILogger<LoggingQueryHandler<TQuery, TResponse>>>()));

        return this;
    }

    /// <summary>
    /// Adds the validation decorator to the query handler chain.
    /// Runs all registered <see cref="IValidator{T}"/> instances before the inner handler.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public QueryHandlerBuilder<TQuery, TResponse, THandler> WithValidation()
    {
        _decoratorFactories.Add((sp, inner) =>
            new ValidationQueryHandler<TQuery, TResponse>(
                inner,
                sp.GetRequiredService<IEnumerable<IValidator<TQuery>>>(),
                sp.GetRequiredService<ILogger<ValidationQueryHandler<TQuery, TResponse>>>()));

        return this;
    }

    /// <summary>
    /// Adds the tracing decorator to the query handler chain.
    /// Creates an <see cref="System.Diagnostics.Activity"/> span around the handler execution.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public QueryHandlerBuilder<TQuery, TResponse, THandler> WithTracing()
    {
        _decoratorFactories.Add((_, inner) =>
            new TracingQueryHandler<TQuery, TResponse>(inner));

        return this;
    }

    /// <summary>
    /// Adds the timeout decorator to the query handler chain.
    /// Enforces the timeout configured via <see cref="TimeoutAttribute"/> on the query type.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public QueryHandlerBuilder<TQuery, TResponse, THandler> WithTimeout()
    {
        _decoratorFactories.Add((_, inner) =>
            new TimeoutQueryHandler<TQuery, TResponse>(inner));

        return this;
    }

    /// <summary>
    /// Finalizes the builder and registers the decorated query handler in the DI container.
    /// Decorators wrap inside-out: the last <c>.With*()</c> call is the outermost layer.
    /// </summary>
    /// <returns>The service collection for further registrations.</returns>
    public IServiceCollection Build()
    {
        _services.AddScoped<THandler>();

        if (_decoratorFactories.Count is 0)
        {
            _services.AddScoped<IQueryHandler<TQuery, TResponse>>(sp =>
                sp.GetRequiredService<THandler>());

            return _services;
        }

        _services.AddScoped<IQueryHandler<TQuery, TResponse>>(sp =>
        {
            IQueryHandler<TQuery, TResponse> handler = sp.GetRequiredService<THandler>();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var factory in _decoratorFactories)
            {
                handler = factory(sp, handler);
            }

            return handler;
        });

        return _services;
    }
}

