namespace Core.CQRS;

/// <summary>
/// Defines a handler for a command that does not return a value.
/// </summary>
/// <typeparam name="TCommand">The type of the command to handle.</typeparam>
/// <seealso cref="ICommand"/>
/// <seealso href="https://www.milanjovanovic.tech/blog/cqrs-pattern-the-way-it-should-have-been-from-the-start">CQRS Pattern - Milan Jovanović</seealso>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the specified command.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Handle(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a handler for a command that returns a <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TCommand">The type of the command to handle.</typeparam>
/// <typeparam name="TResponse">The type of the result returned by the command.</typeparam>
/// <seealso cref="ICommand{TResponse}"/>
/// <seealso href="https://www.milanjovanovic.tech/blog/cqrs-pattern-the-way-it-should-have-been-from-the-start">CQRS Pattern - Milan Jovanović</seealso>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// Handles the specified command and returns a result.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the command result.</returns>
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
}