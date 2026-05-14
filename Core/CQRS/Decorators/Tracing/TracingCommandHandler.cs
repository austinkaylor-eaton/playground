using System.Diagnostics;

namespace Core.CQRS.Decorators.Tracing;

/// <summary>
/// Tracing decorator for command handlers that do not return a value.
/// Starts an <see cref="Activity"/> span around the inner handler execution,
/// recording the command name, type, and success/failure status.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
/// <example>
/// Register the handler with tracing in the DI container:
/// <code>
/// services.AddCommandHandlerWithTracing&lt;DeleteUserCommand, DeleteUserCommandHandler&gt;();
/// </code>
/// </example>
/// <seealso cref="ICommandHandler{TCommand}"/>
/// <seealso cref="CqrsActivitySource"/>
/// <remarks>
/// Initializes a new instance of the <see cref="TracingCommandHandler{TCommand}"/> class.
/// </remarks>
/// <param name="inner">The inner command handler to decorate.</param>
public sealed class TracingCommandHandler<TCommand>(ICommandHandler<TCommand> inner) :
    ICommandHandler<TCommand> where TCommand : ICommand
{
    /// <inheritdoc />
    public async Task Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        using var activity = CqrsActivitySource.Source.StartActivity(
            commandName);

        activity?.SetTag("cqrs.type", "command");
        activity?.SetTag("cqrs.name", commandName);

        try
        {
            await inner.Handle(command, cancellationToken).ConfigureAwait(false);

            activity?.SetStatus(ActivityStatusCode.Ok);
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