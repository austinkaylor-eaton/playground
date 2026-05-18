using Core.CQRS;

namespace Core.Tests.CQRS;

/// <summary>
/// Unit tests for <see cref="IQueryHandler{TQuery,TResponse}"/> interface contracts.
/// </summary>
public class QueryHandlerTests
{
    private sealed record UserResponse(Guid Id, string Name);

    private sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;

    private sealed record GetAllUsersQuery : IQuery<IReadOnlyList<UserResponse>>;

    private sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserResponse>
    {
        public Task<UserResponse> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(new UserResponse(query.UserId, "Buddy"));
        }
    }

    private sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserResponse>>
    {
        public Task<IReadOnlyList<UserResponse>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            IReadOnlyList<UserResponse> users = [new(Guid.NewGuid(), "Alice"), new(Guid.NewGuid(), "Bob")];

            return Task.FromResult(users);
        }
    }

    [Test]
    public async Task Handler_ReturnsExpectedResponse()
    {
        var handler = new GetUserByIdQueryHandler();
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);

        var result = await handler.Handle(query, CancellationToken.None);

        await Assert.That(result.Id).IsEqualTo(userId);
        await Assert.That(result.Name).IsEqualTo("Buddy");
    }

    [Test]
    public async Task Handler_WithCollectionResponse_ReturnsExpectedItems()
    {
        var handler = new GetAllUsersQueryHandler();
        var query = new GetAllUsersQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        await Assert.That(result.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Handler_ImplementsIQueryHandler()
    {
        var handler = new GetUserByIdQueryHandler();

        await Assert.That(handler).IsAssignableTo<IQueryHandler<GetUserByIdQuery, UserResponse>>();
    }

    [Test]
    public async Task Handler_SupportsCancellationToken()
    {
        var handler = new GetUserByIdQueryHandler();
        var query = new GetUserByIdQuery(Guid.NewGuid());
        using var cts = new CancellationTokenSource();

        var result = await handler.Handle(query, cts.Token);

        await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task NonQueryType_IsNotAssignableToIQuery()
    {
        var record = new UnrelatedRecord("test");

        await Assert.That(record).IsNotAssignableTo<IQuery<string>>();
        await Assert.That(record).IsNotAssignableTo<IQuery<UserResponse>>();
    }

    private sealed record UnrelatedRecord(string Name);
}