using Core.CQRS.Validation;
using Microsoft.Extensions.Logging;

namespace Core.CQRS.Decorators.Validation;

/// <summary>
/// Validation decorator for query handlers.
/// Runs all registered validators before delegating to the inner handler.
/// Throws <see cref="ValidationException"/> if any validation failures are found.
/// </summary>
/// <typeparam name="TQuery">The type of query being handled.</typeparam>
/// <typeparam name="TResponse">The type of the query result.</typeparam>
/// <example>
/// Register the handler with validation in the DI container:
/// <code>
/// services.AddValidator&lt;GetUserByIdQuery, GetUserByIdQueryValidator&gt;();
/// services.AddQueryHandlerWithValidation&lt;GetUserByIdQuery, UserResponse, GetUserByIdQueryHandler&gt;();
/// </code>
/// </example>
/// <seealso cref="IQueryHandler{TQuery, TResponse}"/>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidationQueryHandler{TQuery, TResponse}"/> class.
/// </remarks>
/// <param name="inner">The inner query handler to decorate.</param>
/// <param name="validators">The validators for the query.</param>
/// <param name="logger">The logger instance.</param>
public sealed class ValidationQueryHandler<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> inner,
    IEnumerable<IValidator<TQuery>> validators,
    ILogger<ValidationQueryHandler<TQuery, TResponse>> logger) :
    IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await inner.Handle(query, cancellationToken).ConfigureAwait(false);
        }

        var failures = new List<ValidationFailure>();

        foreach (var validator in validators)
        {
            var results = await validator.ValidateAsync(query, cancellationToken).ConfigureAwait(false);

            if (results is { Count: > 0 })
            {
                failures.AddRange(results);
            }
        }

        if (failures is not { Count: > 0 })
        {
            return await inner.Handle(query, cancellationToken).ConfigureAwait(false);
        }

        var queryName = typeof(TQuery).Name;
        Log.ValidationFailed(logger, queryName, failures.Count);

        throw new ValidationException(failures);

    }
}
