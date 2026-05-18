using Core.CQRS;

namespace Core.Tests.CQRS;

/// <summary>
/// Unit tests for <see cref="IQuery{TResponse}"/> interface contracts.
/// </summary>
public class QueryTests
{
    private sealed record UserResponse(Guid Id, string Name);

    private sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;

    private sealed record GetAllUsersQuery : IQuery<IReadOnlyList<UserResponse>>;

    private sealed record UnrelatedRecord(string Name);

    [Test]
    public async Task QueryWithResponse_ImplementsIQueryOfTResponse()
    {
        var query = new GetUserByIdQuery(Guid.NewGuid());

        await Assert.That(query).IsAssignableTo<IQuery<UserResponse>>();
    }

    [Test]
    public async Task QueryWithCollectionResponse_ImplementsIQueryOfCollection()
    {
        var query = new GetAllUsersQuery();

        await Assert.That(query).IsAssignableTo<IQuery<IReadOnlyList<UserResponse>>>();
    }

    [Test]
    public async Task UnrelatedRecord_DoesNotImplementIQuery()
    {
        var record = new UnrelatedRecord("Buddy");

        await Assert.That(record).IsNotAssignableTo<IQuery<UserResponse>>();
    }

    [Test]
    public async Task Query_DoesNotImplementICommand()
    {
        var query = new GetUserByIdQuery(Guid.NewGuid());

        await Assert.That(query).IsNotAssignableTo<ICommand>();
        await Assert.That(query).IsNotAssignableTo<ICommand<UserResponse>>();
    }
}