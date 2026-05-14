namespace Core.CQRS.Decorators.Timeout;

/// <summary>
/// Timeout decorator for command handlers that return a <typeparamref name="TResponse"/>.
/// If the command type is annotated with <see cref="TimeoutAttribute"/>, execution is
/// cancelled after the specified duration and a <see cref="TimeoutException"/> is thrown.
/// If no attribute is present, the decorator is a no-op pass-through.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
/// <typeparam name="TResponse">The type of the command result.</typeparam>
/// <example>
/// <code>
/// [Timeout(timeoutMs: 3000)]
/// public sealed record CreateOrderCommand(string Product) : ICommand&lt;Guid&gt;;
///
/// services.AddCommandHandlerWithTimeout&lt;CreateOrderCommand, Guid, CreateOrderCommandHandler&gt;();
/// </code>
/// </example>
/// <seealso cref="ICommandHandler{TCommand, TResponse}"/>
/// <seealso cref="TimeoutAttribute"/>
/// <remarks>
/// Initializes a new instance of the <see cref="TimeoutCommandHandlerWithResponse{TCommand, TResponse}"/> class.
/// </remarks>
/// <param name="inner">The inner command handler to decorate.</param>
public sealed class TimeoutCommandHandlerWithResponse<TCommand, TResponse>(ICommandHandler<TCommand, TResponse> inner)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private static readonly TimeSpan? ConfiguredTimeout =
        typeof(TCommand).GetCustomAttributes(typeof(TimeoutAttribute), false) is [TimeoutAttribute attr, ..]
            ? attr.Duration
            : null;

    /// <inheritdoc />
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (ConfiguredTimeout is not { } timeout)
        {
            return await inner.Handle(command, cancellationToken).ConfigureAwait(false);
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        try
        {
            return await inner.Handle(command, cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"Command {typeof(TCommand).Name} exceeded the configured timeout of {timeout.TotalMilliseconds}ms.");
        }
    }
}