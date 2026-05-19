namespace Core.CQRS;

/// <summary>
/// Represents a void return type for commands that do not produce a meaningful result.
/// </summary>
/// <remarks>
/// Use <see cref="Unit"/> as the <c>TResponse</c> type parameter for commands
/// that perform side effects without returning a value. This avoids the need for
/// a separate void-specific handler interface.
/// </remarks>
/// <example>
/// <code>
/// // Define a void command:
/// public sealed record DeleteUserCommand(Guid UserId) : ICommand;
///
/// // Implement the handler returning Unit:
/// public sealed class DeleteUserCommandHandler : ICommandHandler&lt;DeleteUserCommand, Unit&gt;
/// {
///     public Task&lt;Unit&gt; Handle(DeleteUserCommand command, CancellationToken cancellationToken)
///     {
///         // Delete user from data store
///         return Unit.Task;
///     }
/// }
/// </code>
/// </example>
public readonly record struct Unit
{
    /// <summary>
    /// Gets the single default value of <see cref="Unit"/>.
    /// </summary>
    public static Unit Value => default;

    /// <summary>
    /// Gets a completed <see cref="Task{Unit}"/> with the default <see cref="Unit"/> value.
    /// </summary>
    /// <example>
    /// <code>
    /// public Task&lt;Unit&gt; Handle(MyCommand command, CancellationToken ct)
    /// {
    ///     // perform side effects...
    ///     return Unit.Task;
    /// }
    /// </code>
    /// </example>
    public static Task<Unit> Task { get; } = System.Threading.Tasks.Task.FromResult(default(Unit));
}

