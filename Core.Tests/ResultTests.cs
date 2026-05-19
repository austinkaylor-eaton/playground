
namespace Core.Tests;

/// <summary>
/// Unit tests for <see cref="Result{TSuccess, TError}"/> and <see cref="Result"/> factory methods.
/// </summary>
public class ResultTests
{
    [Test]
    public async Task Success_CreatesSuccessfulResult()
    {
        var result = Result.Success<int, TestError>(42);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsEqualTo(42);
        await Assert.That(result.Error).IsNull();
    }

    [Test]
    public async Task Failure_CreatesFailedResult()
    {
        var error = new TestError("Something went wrong");
        var result = Result.Failure<int, TestError>(error);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error).IsEqualTo(error);
    }

    [Test]
    public async Task Success_WithNullValue_ThrowsArgumentNullException()
    {
        var action = () => Result.Success<string?, TestError>(null);

        await Assert.That(action).ThrowsException().WithMessageMatching("*non-null*");
    }

    [Test]
    public async Task Failure_WithNullError_ThrowsArgumentNullException()
    {
        var action = () => Result.Failure<int, TestError>(null!);

        await Assert.That(action).ThrowsException().WithMessageMatching("*non-null*");
    }

    [Test]
    public async Task DefaultResult_HasNullError_IsConsideredSuccess()
    {
        var result = default(Result<int, TestError>);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Error).IsNull();
    }

    [Test]
    public async Task Map_OnSuccess_TransformsValue()
    {
        var result = Result.Success<int, TestError>(10);

        var mapped = result.Map(v => v * 2);

        await Assert.That(mapped.IsSuccess).IsTrue();
        await Assert.That(mapped.Value).IsEqualTo(20);
    }

    [Test]
    public async Task Map_OnFailure_PropagatesError()
    {
        var error = new TestError("fail");
        var result = Result.Failure<int, TestError>(error);

        var mapped = result.Map(v => v * 2);

        await Assert.That(mapped.IsSuccess).IsFalse();
        await Assert.That(mapped.Error).IsEqualTo(error);
    }

    [Test]
    public async Task Map_WithNullFunc_ThrowsArgumentNullException()
    {
        var result = Result.Success<int, TestError>(1);
        Func<int, string> nullFunc = null!;

        var action = () => result.Map(nullFunc);

        await Assert.That(action).ThrowsException();
    }

    [Test]
    public async Task Bind_OnSuccess_ChainsOperation()
    {
        var result = Result.Success<int, TestError>(5);

        var bound = result.Bind(v => Result.Success<string, TestError>($"value={v}"));

        await Assert.That(bound.IsSuccess).IsTrue();
        await Assert.That(bound.Value).IsEqualTo("value=5");
    }

    [Test]
    public async Task Bind_OnFailure_PropagatesError()
    {
        var error = new TestError("original");
        var result = Result.Failure<int, TestError>(error);

        var bound = result.Bind(v => Result.Success<string, TestError>($"value={v}"));

        await Assert.That(bound.IsSuccess).IsFalse();
        await Assert.That(bound.Error).IsEqualTo(error);
    }

    [Test]
    public async Task Bind_OnSuccess_WhenBindReturnsFailure_PropagatesBindError()
    {
        var result = Result.Success<int, TestError>(5);
        var bindError = new TestError("bind failed");

        var bound = result.Bind(_ => Result.Failure<string, TestError>(bindError));

        await Assert.That(bound.IsSuccess).IsFalse();
        await Assert.That(bound.Error).IsEqualTo(bindError);
    }

    [Test]
    public async Task Bind_WithNullFunc_ThrowsArgumentNullException()
    {
        var result = Result.Success<int, TestError>(1);
        Func<int, Result<string, TestError>> nullFunc = null!;

        var action = () => result.Bind(nullFunc);

        await Assert.That(action).ThrowsException();
    }

    private sealed record TestError(string Message) : ResultError(Message);
}




