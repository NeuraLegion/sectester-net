using System.Net;
using System.Net.Sockets;
using SecTester.Core.Tests.Fixtures;

namespace SecTester.Core.Tests.RetryStrategies;

public class ExponentialBackoffRetryStrategyTests : IDisposable
{
  private readonly ExponentialBackoffIRetryStrategy _sut;
  private readonly IMockInterface _mockInterface;

  public ExponentialBackoffRetryStrategyTests()
  {
    var options = new ExponentialBackoffOptions(2, 1);
    _sut = new ExponentialBackoffIRetryStrategy(options);
    _mockInterface = Substitute.For<IMockInterface>();
  }

  public void Dispose()
  {
    _mockInterface.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task Acquire_NotThrowError_DoesNotRetry()
  {
    // arrange
    _mockInterface.Execute().Returns(Task.FromResult<string>("result"));

    // act
    var act = () => _sut.Acquire(() => _mockInterface.Execute());

    // assert
    await act.Should().NotThrowAsync();
    await _mockInterface.Received(1).Execute();
  }

  [Fact]
  public async Task Acquire_NotThrowError_ReturnsResultOfExecutionImmediately()
  {
    // arrange
    _mockInterface.Execute().Returns(Task.FromResult<string>("result"));

    // act
    var result = await _sut.Acquire(() => _mockInterface.Execute());

    // assert
    result.Should().Be("result");
  }

  [Fact]
  public async Task Acquire_Canceled_PreventsRetries()
  {
    // arrange
    var error = new Exception("Unhandled error");
    var cts = new CancellationTokenSource();
    cts.Cancel();
    _mockInterface.Execute().ThrowsAsync(error);

    // act
    var act = () => _sut.Acquire(() => _mockInterface.Execute(), cts.Token);

    // assert
    await act.Should().ThrowAsync<OperationCanceledException>();
    await _mockInterface.DidNotReceive().Execute();
  }

  [Fact]
  public async Task Acquire_NotRetryableError_PreventsRetries()
  {
    // arrange
    var error = new Exception("Unhandled error");
    _mockInterface.Execute().ThrowsAsync(error);

    // act
    var act = () => _sut.Acquire(() => _mockInterface.Execute());

    // assert
    await act.Should().ThrowAsync<Exception>();
    await _mockInterface.Received(1).Execute();
  }

  [Fact]
  public async Task Acquire_MaxDepthIsNotReached_ReturnsResult()
  {
    // arrange
    var error = new HttpStatusException("Unhandled error", HttpStatusCode.InternalServerError);
    _mockInterface.Execute().Returns(_ => throw error, _ => throw error,
      _ => "result");

    // act
    var result = await _sut.Acquire(() => _mockInterface.Execute());

    // assert
    result.Should().Be("result");
    await _mockInterface.Received(3).Execute();
  }

  public static IEnumerable<object[]> Exceptions
  {
    get
    {
      var list = new List<object[]>
      {
        new object[] { new SocketException() }
      };
      list.AddRange(Enumerable.Repeat(500, 12).Select(x => new[]
      {
        new HttpStatusException($"Status code: {x + 1}", (HttpStatusCode)(x + 1))
      }));

      return list;
    }
  }

  [Theory]
  [MemberData(nameof(Exceptions))]
  public async Task Acquire_RetryableError_Retries(Exception input)
  {
    // arrange
    _mockInterface.Execute().ThrowsAsync(input);

    // act
    var act = () => _sut.Acquire(() => _mockInterface.Execute());

    // assert
    await act.Should().ThrowAsync<Exception>();
    await _mockInterface.Received(3).Execute();
  }
}
