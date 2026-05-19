using Core.CQRS;

namespace Core.Tests.CQRS;

/// <summary>
/// Unit tests for <see cref="ICommand"/> and <see cref="ICommand{TResponse}"/> interface contracts.
/// </summary>
public class CommandTests
{
    private sealed record CreatePetCommand(string Name) : ICommand<Guid>;

    private sealed record DeletePetCommand(Guid Id) : ICommand;

    private sealed record UnrelatedRecord(string Name);

    [Test]
    public async Task CommandWithResponse_ImplementsICommandOfTResponse()
    {
        var command = new CreatePetCommand("Buddy");

        await Assert.That(command).IsAssignableTo<ICommand<Guid>>();
    }

    [Test]
    public async Task VoidCommand_ImplementsICommandOfUnit()
    {
        var command = new DeletePetCommand(Guid.NewGuid());

        await Assert.That(command).IsAssignableTo<ICommand<Unit>>();
    }

    [Test]
    public async Task VoidCommand_ImplementsICommand()
    {
        var command = new DeletePetCommand(Guid.NewGuid());

        await Assert.That(command).IsAssignableTo<ICommand>();
    }

    [Test]
    public async Task UnrelatedRecord_DoesNotImplementICommand()
    {
        var record = new UnrelatedRecord("Buddy");

        await Assert.That(record).IsNotAssignableTo<ICommand>();
        await Assert.That(record).IsNotAssignableTo<ICommand<Unit>>();
    }

    [Test]
    public async Task CommandWithResponse_DoesNotImplementVoidICommand()
    {
        var command = new CreatePetCommand("Buddy");

        await Assert.That(command).IsNotAssignableTo<ICommand>();
    }
}