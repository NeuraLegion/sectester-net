namespace SecTester.Bus.Tests.Dispatchers;

public class DefaultRmqConnectionManagerTests : IDisposable
{
  private readonly IRmqConnectionManager _manager;
  private readonly IModel _channel = Substitute.For<IModel>();
  private readonly IConnection _connection = Substitute.For<IConnection>();
  private readonly IConnectionFactory _connectionFactory = Substitute.For<IConnectionFactory>();
  private readonly ILogger<DefaultRmqConnectionManager> _logger = Substitute.For<ILogger<DefaultRmqConnectionManager>>();
  private readonly IRetryStrategy _retryStrategy = Substitute.For<IRetryStrategy>();

  public DefaultRmqConnectionManagerTests()
  {
    _connectionFactory.CreateConnection().Returns(_connection);
    _connection.CreateModel().Returns(_channel);
    _retryStrategy.Acquire(Arg.Any<Func<Task<IConnection>>>()).Returns(x => x.ArgAt<Func<Task<IConnection>>>(0).Invoke());
    _connection.Endpoint.Returns(new AmqpTcpEndpoint
    {
      HostName = "localhost"
    });
    _manager = new DefaultRmqConnectionManager(_connectionFactory, _logger, _retryStrategy);
  }

  public void Dispose()
  {
    _retryStrategy.ClearSubstitute();
    _connection.ClearSubstitute();
    _channel.ClearSubstitute();
    _connectionFactory.ClearSubstitute();
    _logger.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public void CreateChannel_Connected_CreatesChannel()
  {
    // arrange
    _manager.TryConnect();
    _connection.IsOpen.Returns(true);

    // act
    var channel = _manager.CreateChannel();

    // assert
    channel.Should().NotBeNull();
    _connection.Received(1).CreateModel();
  }

  [Fact]
  public void CreateChannel_NotConnected_ThrowsError()
  {
    // arrange
    _manager.TryConnect();
    _connection.IsOpen.Returns(false);

    // act
    var act = () => _manager.CreateChannel();

    // assert
    act.Should().Throw<InvalidOperationException>();
  }

  [Fact]
  public void Consume_Connected_CreatesChannel()
  {
    // arrange
    _manager.TryConnect();
    _connection.IsOpen.Returns(true);

    // act
    var result = _manager.CreateConsumer(_channel);

    // assert
    result.Should().NotBeNull();
  }

  [Fact]
  public void CreateConsumer_NotConnected_ThrowsError()
  {
    // arrange
    _manager.TryConnect();
    _connection.IsOpen.Returns(false);

    // act
    var act = () => _manager.CreateConsumer(_channel);

    // assert
    act.Should().Throw<InvalidOperationException>();
  }

  [Fact]
  public void TryConnect_NotConnected_ConnectsToHost()
  {
    // arrange
    _connection.IsOpen.Returns(false);

    // act
    _manager.TryConnect();

    // assert
    _connectionFactory.Received().CreateConnection();
  }

  [Fact]
  public void TryConnect_Connected_ReturnsControl()
  {
    // arrange
    _connection.IsOpen.Returns(false, true);

    // act
    _manager.TryConnect();
    _manager.TryConnect();

    // assert
    _connectionFactory.Received(1).CreateConnection();
  }

  [Fact]
  public void Connect_ConnectsToHost()
  {
    // arrange
    _connection.IsOpen.Returns(false, true);

    // act
    _manager.Connect();

    // assert
    _connectionFactory.Received().CreateConnection();
  }

  [Fact]
  public void Connect_ConnectionError_Retries()
  {
    // arrange
    _connection.IsOpen.Returns(false, true);
    _connectionFactory.CreateConnection().Throws(new SocketException());

    // act
    var act = () => _manager.Connect();

    // assert
    act.Should().Throw<SocketException>();
    _retryStrategy.Received().Acquire(Arg.Any<Func<Task<IConnection>>>());
  }

  [Fact]
  public void Connect_ReconnectsOnShutdown()
  {
    // arrange
    var shutdownEventArgs = new ShutdownEventArgs(ShutdownInitiator.Peer, 1, "something went wrong");
    _connection.IsOpen.Returns(true);

    // act
    _manager.Connect();
    _connection.ConnectionShutdown +=
      Raise.EventWith(new object(), shutdownEventArgs);

    // assert
    _connectionFactory.Received(2).CreateConnection();
  }

  [Fact]
  public void Constructor_ReconnectsOnException()
  {
    // arrange
    var exception = new Exception("something went wrong");
    var callbackExceptionEventArgs = new CallbackExceptionEventArgs(exception);

    _connection.IsOpen.Returns(true);

    // act
    _manager.Connect();
    _connection.CallbackException += Raise.EventWith(new object(), callbackExceptionEventArgs);

    // assert
    _connectionFactory.Received(2).CreateConnection();
  }

  [Fact]
  public void Constructor_ReconnectsWhenConnectionBlocked()
  {
    // arrange
    var connectionBlockedEventArgs = new ConnectionBlockedEventArgs("something went wrong");

    _connection.IsOpen.Returns(true);

    // act
    _manager.Connect();
    _connection.ConnectionBlocked += Raise.EventWith(new object(), connectionBlockedEventArgs);

    // assert
    _connectionFactory.Received(2).CreateConnection();
  }

  [Fact]
  public void Dispose_Connected_RemovesSubscriptions()
  {
    // arrange
    _manager.Connect();
    _connection.IsOpen.Returns(true);

    // act
    _manager.Dispose();

    // assert
    _connection.Received().ConnectionBlocked -= Arg.Any<EventHandler<ConnectionBlockedEventArgs>>();
    _connection.Received().CallbackException -= Arg.Any<EventHandler<CallbackExceptionEventArgs>>();
    _connection.Received().ConnectionShutdown -= Arg.Any<EventHandler<ShutdownEventArgs>>();
  }

  [Fact]
  public void Dispose_NotConnected_DoesNothing()
  {
    // arrange
    _connection.IsOpen.Returns(false);

    // act
    _manager.Dispose();

    // assert
    _connection.DidNotReceive().Close();
  }

  [Fact]
  public void Dispose_Connected_ClosesConnection()
  {
    // arrange
    _manager.Connect();
    _connection.IsOpen.Returns(true);

    // act
    _manager.Dispose();

    // assert
    _connection.Received().Close();
  }
}
