using System.Diagnostics;

namespace Core.CQRS.Decorators.Tracing;

/// <summary>
/// Tracing decorator for command handlers that return a <typeparamref name="TResponse"/>.
/// Starts an <see cref="Activity"/> span around the inner handler execution,
/// recording the command name, type, and success/failure status.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
/// <typeparam name="TResponse">The type of the command result.</typeparam>
/// <example>
/// Register the handler with tracing in the DI container:
/// <code>
/// services.AddCommandHandlerWithTracing&lt;CreateUserCommand, Guid, CreateUserCommandHandler&gt;();
/// </code>
/// </example>
/// <seealso cref="ICommandHandler{TCommand, TResponse}"/>
/// <seealso cref="CqrsActivitySource"/>
/// <remarks>
/// Initializes a new instance of the <see cref="TracingCommandHandlerWithResponse{TCommand, TResponse}"/> class.
/// </remarks>
/// <param name="inner">The inner command handler to decorate.</param>
public sealed class TracingCommandHandlerWithResponse<TCommand, TResponse>(ICommandHandler<TCommand, TResponse> inner)
    : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        using var activity = CqrsActivitySource.Source.StartActivity(
            commandName);

        activity?.SetTag("cqrs.type", "command");
        activity?.SetTag("cqrs.name", commandName);

        try
        {
            var result = await inner.Handle(command, cancellationToken).ConfigureAwait(false);

            activity?.SetStatus(ActivityStatusCode.Ok);

            return result;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent("exception",
                tags: new ActivityTagsCollection
                {
                    { "exception.type", ex.GetType().FullName },
                    { "exception.message", ex.Message },
                }));

            throw;
        }
    }
}