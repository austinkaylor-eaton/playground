namespace Core.CQRS.Decorators.Timeout;

/// <summary>
/// Timeout decorator for command handlers that do not return a value.
/// If the command type is annotated with <see cref="TimeoutAttribute"/>, execution is
/// cancelled after the specified duration and a <see cref="TimeoutException"/> is thrown.
/// If no attribute is present, the decorator is a no-op pass-through.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
/// <example>
/// <code>
/// [Timeout(timeoutMs: 5000)]
/// public sealed record SendEmailCommand(string To, string Body) : ICommand;
///
/// services.AddCommandHandlerWithTimeout&lt;SendEmailCommand, SendEmailCommandHandler&gt;();
/// </code>
/// </example>
/// <seealso cref="ICommandHandler{TCommand}"/>
/// <seealso cref="TimeoutAttribute"/>
/// <remarks>
/// Initializes a new instance of the <see cref="TimeoutCommandHandler{TCommand}"/> class.
/// </remarks>
/// <param name="inner">The inner command handler to decorate.</param>
public sealed class TimeoutCommandHandler<TCommand>(ICommandHandler<TCommand> inner) : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private static readonly TimeSpan? ConfiguredTimeout =
        typeof(TCommand).GetCustomAttributes(typeof(TimeoutAttribute), false) is [TimeoutAttribute attr, ..]
            ? attr.Duration
            : null;

    /// <inheritdoc />
    public async Task Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (ConfiguredTimeout is not { } timeout)
        {
            await inner.Handle(command, cancellationToken).ConfigureAwait(false);

            return;
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        try
        {
            await inner.Handle(command, cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"Command {typeof(TCommand).Name} exceeded the configured timeout of {timeout.TotalMilliseconds}ms.");
        }
    }
}