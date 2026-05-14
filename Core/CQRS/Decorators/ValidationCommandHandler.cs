using Core.Validation;
using Microsoft.Extensions.Logging;

namespace Core.CQRS.Decorators;

/// <summary>
/// Validation decorator for command handlers that do not return a value.
/// Runs all registered validators before delegating to the inner handler.
/// Throws <see cref="ValidationException"/> if any validation failures are found.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
/// <example>
/// Register the handler with validation in the DI container:
/// <code>
/// services.AddValidator&lt;DeleteUserCommand, DeleteUserCommandValidator&gt;();
/// services.AddCommandHandlerWithValidation&lt;DeleteUserCommand, DeleteUserCommandHandler&gt;();
/// </code>
/// </example>
/// <seealso cref="ICommandHandler{TCommand}"/>
public sealed class ValidationCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> inner;
    private readonly IEnumerable<IValidator<TCommand>> validators;
    private readonly ILogger<ValidationCommandHandler<TCommand>> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationCommandHandler{TCommand}"/> class.
    /// </summary>
    /// <param name="inner">The inner command handler to decorate.</param>
    /// <param name="validators">The validators for the command.</param>
    /// <param name="logger">The logger instance.</param>
    public ValidationCommandHandler(
        ICommandHandler<TCommand> inner,
        IEnumerable<IValidator<TCommand>> validators,
        ILogger<ValidationCommandHandler<TCommand>> logger)
    {
        this.inner = inner;
        this.validators = validators;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            await inner.Handle(command, cancellationToken).ConfigureAwait(false);
            return;
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

        if (failures is { Count: > 0 })
        {
            var commandName = typeof(TCommand).Name;
            CQRS.Log.ValidationFailed(logger, commandName, failures.Count);

            throw new ValidationException(failures);
        }

        await inner.Handle(command, cancellationToken).ConfigureAwait(false);
    }
}
