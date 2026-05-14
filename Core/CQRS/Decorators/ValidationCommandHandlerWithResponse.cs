using Core.Validation;
using Microsoft.Extensions.Logging;

namespace Core.CQRS.Decorators;

/// <summary>
/// Validation decorator for command handlers that return a <typeparamref name="TResponse"/>.
/// Runs all registered validators before delegating to the inner handler.
/// Throws <see cref="ValidationException"/> if any validation failures are found.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
/// <typeparam name="TResponse">The type of the command result.</typeparam>
/// <example>
/// Register the handler with validation in the DI container:
/// <code>
/// services.AddValidator&lt;CreateUserCommand, CreateUserCommandValidator&gt;();
/// services.AddCommandHandlerWithValidation&lt;CreateUserCommand, Guid, CreateUserCommandHandler&gt;();
/// </code>
/// </example>
/// <seealso cref="ICommandHandler{TCommand, TResponse}"/>
public sealed class ValidationCommandHandlerWithResponse<TCommand, TResponse>
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ICommandHandler<TCommand, TResponse> inner;
    private readonly IEnumerable<IValidator<TCommand>> validators;
    private readonly ILogger<ValidationCommandHandlerWithResponse<TCommand, TResponse>> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationCommandHandlerWithResponse{TCommand, TResponse}"/> class.
    /// </summary>
    /// <param name="inner">The inner command handler to decorate.</param>
    /// <param name="validators">The validators for the command.</param>
    /// <param name="logger">The logger instance.</param>
    public ValidationCommandHandlerWithResponse(
        ICommandHandler<TCommand, TResponse> inner,
        IEnumerable<IValidator<TCommand>> validators,
        ILogger<ValidationCommandHandlerWithResponse<TCommand, TResponse>> logger)
    {
        this.inner = inner;
        this.validators = validators;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await inner.Handle(command, cancellationToken).ConfigureAwait(false);
        }

        var failures = new List<ValidationFailure>();

        foreach (var validator in validators)
        {
            var results = await validator.ValidateAsync(command, cancellationToken).ConfigureAwait(false);

            if (results is { Count: > 0 })
            {
                failures.AddRange(results);
            }
        }

        if (failures is not { Count: > 0 })
        {
            return await inner.Handle(command, cancellationToken).ConfigureAwait(false);
        }

        var commandName = typeof(TCommand).Name;
        Log.ValidationFailed(logger, commandName, failures.Count);

        throw new ValidationException(failures);

    }
}
