namespace Core.CQRS;

/// <summary>
/// Defines a handler for a command that returns a <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TCommand">The type of the command to handle.</typeparam>
/// <typeparam name="TResponse">The type of the result returned by the command.</typeparam>
/// <remarks>
/// For commands that do not return a meaningful value, use <see cref="Unit"/> as the response type.
/// </remarks>
/// <example>
/// <code>
/// // Handler that returns a response:
/// public sealed class CreateUserCommandHandler
///     : ICommandHandler&lt;CreateUserCommand, Guid&gt;
/// {
///     public async Task&lt;Guid&gt; Handle(CreateUserCommand command, CancellationToken ct)
///     {
///         var id = Guid.NewGuid();
///         // persist the new user...
///         return id;
///     }
/// }
///
/// // Void handler using Unit:
/// public sealed class DeleteUserCommandHandler
///     : ICommandHandler&lt;DeleteUserCommand, Unit&gt;
/// {
///     public Task&lt;Unit&gt; Handle(DeleteUserCommand command, CancellationToken cancellationToken)
///     {
///         // Delete user from data store
///         return Unit.Task;
///     }
/// }
/// </code>
/// </example>
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