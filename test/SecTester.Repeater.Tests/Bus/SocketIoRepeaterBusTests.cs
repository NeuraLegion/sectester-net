namespace SecTester.Repeater.Tests.Bus;

public sealed class SocketIoRepeaterBusTests : IDisposable
{
  private static readonly string RepeaterId = "g5MvgM74sweGcK1U6hvs76";
  private static readonly Uri Url = new("http://example.com");
  private static readonly SocketIoRepeaterBusOptions Options = new(Url);

  private readonly ISocketIoConnection _connection = Substitute.For<ISocketIoConnection>();
  private readonly ILogger<IRepeaterBus> _logger = Substitute.For<ILogger<IRepeaterBus>>();
  private readonly ISocketIoMessage _socketIoMessage = Substitute.For<ISocketIoMessage>();
  private readonly SocketIoRepeaterBus _sut;

  public SocketIoRepeaterBusTests()
  {
    _sut = new SocketIoRepeaterBus(Options, _connection, _logger);
  }

  public void Dispose()
  {
    _socketIoMessage.ClearSubstitute();
    _connection.ClearSubstitute();
    _logger.ClearSubstitute();

    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task RequestReceived_ExecutesHandler()
  {
    // arrange
    var result = new OutgoingResponse
    {
      StatusCode = 204
    };
    _connection.Connect().Returns(Task.CompletedTask);
    _socketIoMessage.GetValue<Dictionary<object, object>>().Returns(new Dictionary<object, object>()
    {
      {"url",  Url.ToString()}
    });
    _connection.On("request", Arg.Invoke(_socketIoMessage));
    _sut.RequestReceived += _ => Task.FromResult(result);

    // act
    await _sut.Connect();

    // assert
    await _socketIoMessage.Received().CallbackAsync(Arg.Any<CancellationToken>(), result);
  }

  [Fact]
  public async Task ErrorOccurred_ExecutesHandler()
  {
    // arrange
    const string msg = "Something went wrong";
    Exception? result = null;
    _connection.Connect().Returns(Task.CompletedTask);
    _socketIoMessage.GetValue<RepeaterError>().Returns(new RepeaterError { Message = msg });
    _connection.On("error", Arg.Invoke(_socketIoMessage));
    _sut.ErrorOccurred += err =>
    {
      result = err;
    };

    // act
    await _sut.Connect();

    // assert
    result.Should().BeEquivalentTo(new { Message = msg });
  }

  [Fact]
  public async Task UpgradeAvailable_ExecutesHandler()
  {
    // arrange
    Version? result = null;
    _connection.Connect().Returns(Task.CompletedTask);
    _socketIoMessage.GetValue<RepeaterVersion>().Returns(new RepeaterVersion { Version = "1.1.1" });
    _connection.On("update-available", Arg.Invoke(_socketIoMessage));
    _sut.UpgradeAvailable += version =>
    {
      result = version;
    };

    // act
    await _sut.Connect();

    // assert
    result.Should().BeOfType<Version>();
  }

  [Fact]
  public async Task Connect_Success()
  {
    // arrange
    _connection.Connect().Returns(Task.CompletedTask);

    // act
    await _sut.Connect();

    // assert
    await _connection.Received().Connect();
  }

  [Fact]
  public async Task Connect_AlreadyConnected_DoNothing()
  {
    // arrange
    _connection.Connected.Returns(true);

    // act
    await _sut.Connect();

    // assert
    await _connection.DidNotReceive().Connect();
  }

  [Fact]
  public async Task Deploy_Success()
  {
    // arrange
    _connection.On("deployed", Arg.Invoke(_socketIoMessage));

    // act
    await _sut.Deploy(RepeaterId);

    // assert
    await _connection.Received().EmitAsync("deploy", Arg.Is<RepeaterInfo>(x => x.RepeaterId.Equals(RepeaterId, StringComparison.OrdinalIgnoreCase)));
  }

  [Fact]
  public async Task Deploy_GivenCancellationToken_ThrowsError()
  {
    // arrange
    using var cancellationTokenSource = new CancellationTokenSource();
    cancellationTokenSource.Cancel();

    // act
    var act = () => _sut.Deploy(RepeaterId, cancellationTokenSource.Token);

    // assert
    await act.Should().ThrowAsync<OperationCanceledException>();
  }

  [Fact]
  public async Task DisposeAsync_Success()
  {
    // arrange
    _connection.Connected.Returns(true);

    // act
    await _sut.DisposeAsync();

    // assert
    await _connection.Received().Disconnect();
    _connection.Received().Dispose();
  }

  [Fact]
  public async Task DisposeAsync_NotConnectedYet_Success()
  {
    // act
    await _sut.DisposeAsync();

    // assert
    await _connection.DidNotReceive().Disconnect();
    _connection.Received().Dispose();
  }
}
