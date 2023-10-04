namespace SecTester.Repeater.Tests.Bus;

public sealed class SocketIoRepeaterBusTests : IDisposable
{
  private static readonly string RepeaterId = "g5MvgM74sweGcK1U6hvs76";
  private static readonly Uri Url = new("http://example.com");
  private static readonly SocketIoRepeaterBusOptions Options = new(Url);

  private readonly ISocketIoClient _client = Substitute.For<ISocketIoClient>();
  private readonly ITimerProvider _heartbeat = Substitute.For<ITimerProvider>();
  private readonly ILogger<IRepeaterBus> _logger = Substitute.For<ILogger<IRepeaterBus>>();
  private readonly ISocketIoResponse _socketIoResponse = Substitute.For<ISocketIoResponse>();
  private readonly SocketIoRepeaterBus _sut;

  public SocketIoRepeaterBusTests()
  {
    _sut = new SocketIoRepeaterBus(Options, _client, _heartbeat, _logger);
  }

  public void Dispose()
  {
    _socketIoResponse.ClearSubstitute();
    _client.ClearSubstitute();
    _heartbeat.ClearSubstitute();
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
    _client.Connect().Returns(Task.CompletedTask);
    _socketIoResponse.GetValue<IncomingRequest>().Returns(new IncomingRequest(Url));
    _client.On("request", Arg.Invoke(_socketIoResponse));
    _sut.RequestReceived += _ => Task.FromResult(result);

    // act
    await _sut.Connect();

    // assert
    await _socketIoResponse.Received().CallbackAsync(Arg.Any<CancellationToken>(), result);
  }

  [Fact]
  public async Task ErrorOccurred_ExecutesHandler()
  {
    // arrange
    const string msg = "Something went wrong";
    Exception? result = null;
    _client.Connect().Returns(Task.CompletedTask);
    _socketIoResponse.GetValue<SocketIoRepeaterBus.RepeaterError>().Returns(new SocketIoRepeaterBus.RepeaterError(msg));
    _client.On("error", Arg.Invoke(_socketIoResponse));
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
    _client.Connect().Returns(Task.CompletedTask);
    _socketIoResponse.GetValue<SocketIoRepeaterBus.RepeaterVersion>().Returns(new SocketIoRepeaterBus.RepeaterVersion("1.1.1"));
    _client.On("update-available", Arg.Invoke(_socketIoResponse));
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
    _client.Connect().Returns(Task.CompletedTask);

    // act
    await _sut.Connect();

    // assert
    await _client.Received().Connect();
  }

  [Fact]
  public async Task Connect_AlreadyConnected_DoNothing()
  {
    // arrange
    _client.Connected.Returns(true);

    // act
    await _sut.Connect();

    // assert
    await _client.DidNotReceive().Connect();
  }

  [Fact]
  public async Task Connect_SchedulesPing()
  {
    // arrange
    _client.Connect().Returns(Task.CompletedTask);

    // act
    await _sut.Connect();

    // assert
    _heartbeat.Received().Elapsed += Arg.Any<ElapsedEventHandler>();
    _heartbeat.Received().Start();
  }

  [Fact]
  public async Task Connect_ShouldSendPingMessage()
  {
    // arrange
    var elapsedEventArgs = EventArgs.Empty as ElapsedEventArgs;
    _client.Connect().Returns(Task.CompletedTask);
    await _sut.Connect();

    // act
    _heartbeat.Elapsed += Raise.Event<ElapsedEventHandler>(new object(), elapsedEventArgs);

    // assert
    _heartbeat.Interval.Should().BeGreaterOrEqualTo(10_000);
    await _client.Received(2).EmitAsync("ping");
  }

  [Fact]
  public async Task Deploy_Success()
  {
    // arrange
    _client.On("deployed", Arg.Invoke(_socketIoResponse));

    // act
    await _sut.Deploy(RepeaterId);

    // assert
    await _client.Received().EmitAsync("deploy", Arg.Is<SocketIoRepeaterBus.RepeaterInfo>(x => x.RepeaterId.Equals(RepeaterId, StringComparison.OrdinalIgnoreCase)));
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
    _client.Connected.Returns(true);

    // act
    await _sut.DisposeAsync();

    // assert
    await _client.Received().Disconnect();
    _client.Received().Dispose();
  }

  [Fact]
  public async Task DisposeAsync_NotConnectedYet_Success()
  {
    // act
    await _sut.DisposeAsync();

    // assert
    await _client.DidNotReceive().Disconnect();
    _client.Received().Dispose();
  }

  [Fact]
  public async Task DisposeAsync_StopsPingMessages()
  {
    // arrange
    _client.Connected.Returns(true);

    // act
    await _sut.DisposeAsync();

    // assert
    _heartbeat.Received().Stop();
  }
}
