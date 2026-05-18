using Core.CQRS;

namespace Core.Tests.CQRS;

/// <summary>
/// Unit tests for <see cref="ICommandHandler{TCommand,TResponse}"/> interface contracts.
/// </summary>
public class CommandHandlerTests
{
    private sealed record CreatePetCommand(string Name) : ICommand<Guid>;

    private sealed record DeletePetCommand(Guid Id) : ICommand;

    private sealed class CreatePetCommandHandler : ICommandHandler<CreatePetCommand, Guid>
    {
        public Task<Guid> Handle(CreatePetCommand command, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();

            return Task.FromResult(id);
        }
    }

    private sealed class DeletePetCommandHandler : ICommandHandler<DeletePetCommand, Unit>
    {
        public Task<Unit> Handle(DeletePetCommand command, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }

    [Test]
    public async Task Handler_WithResponseType_ReturnsExpectedValue()
    {
        var handler = new CreatePetCommandHandler();
        var command = new CreatePetCommand("Buddy");

        var result = await handler.Handle(command, CancellationToken.None);

        await Assert.That(result).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task Handler_WithUnitResponse_ReturnsUnit()
    {
        var handler = new DeletePetCommandHandler();
        var command = new DeletePetCommand(Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        await Assert.That(result).IsEqualTo(Unit.Value);
    }

    [Test]
    public async Task Handler_ImplementsICommandHandler()
    {
        var handler = new CreatePetCommandHandler();

        await Assert.That(handler).IsAssignableTo<ICommandHandler<CreatePetCommand, Guid>>();
    }

    [Test]
    public async Task VoidHandler_ImplementsICommandHandler()
    {
        var handler = new DeletePetCommandHandler();

        await Assert.That(handler).IsAssignableTo<ICommandHandler<DeletePetCommand, Unit>>();
    }

    [Test]
    public async Task Handler_SupportsCancellationToken()
    {
        var handler = new CreatePetCommandHandler();
        var command = new CreatePetCommand("Buddy");
        using var cts = new CancellationTokenSource();

        var result = await handler.Handle(command, cts.Token);

        await Assert.That(result).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task NonCommandType_IsNotAssignableToICommand()
    {
        var pet = new Pet("Buddy", "Bud");

        await Assert.That(pet).IsNotAssignableTo<ICommand>();
        await Assert.That(pet).IsNotAssignableTo<ICommand<Unit>>();
        await Assert.That(pet).IsNotAssignableTo<ICommand<Guid>>();
    }

    private sealed record Pet(string Name, string NickName);
}