using SecTester.Bus.Tests.Fixtures;

namespace SecTester.Bus.Tests.Dispatchers;

public class RmqEventBusTests : IDisposable
{
  private readonly AsyncEventingBasicConsumer _basicConsumer;
  private readonly RmqEventBus _bus;
  private readonly IModel _channel;
  private readonly RmqConnectionManager _connectionManager;
  private readonly ILogger _logger;
  private readonly RmqEventBusOptions _options;
  private readonly AsyncEventingBasicConsumer _replyConsumer;
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly MessageSerializer _messageSerializer;

  public RmqEventBusTests()
  {
    _connectionManager = Substitute.For<RmqConnectionManager>();
    _messageSerializer = Substitute.For<DefaultMessageSerializer>();
    _logger = Substitute.For<ILogger>();
    _scopeFactory = Substitute.For<IServiceScopeFactory>();
    _channel = Substitute.For<IModel>();

    _basicConsumer = new AsyncEventingBasicConsumer(_channel);
    _replyConsumer = new AsyncEventingBasicConsumer(_channel);

    _connectionManager.CreateChannel().Returns(_channel);
    _connectionManager.CreateConsumer(_channel).Returns(_basicConsumer, _replyConsumer);

    _options = new RmqEventBusOptions("amqp://localhost:5672", Exchange: "event-bus", ClientQueue: "Agent", AppQueue: "App");
    _bus = new RmqEventBus(_options, _connectionManager, _logger, _scopeFactory, _messageSerializer);
  }

  public void Dispose()
  {
    _messageSerializer.ClearSubstitute();
    _connectionManager.ClearSubstitute();
    _channel.ClearSubstitute();
    _logger.ClearSubstitute();
    _scopeFactory.ClearSubstitute();

    GC.SuppressFinalize(this);
  }

  [Fact]
  public void Constructor_BindsExchangesToQueue()
  {
    // assert
    _channel.Received().ExchangeDeclare(_options.Exchange, "direct", true);
    _channel.Received().QueueDeclare(_options.ClientQueue, exclusive: false, autoDelete: true, durable: true);
    _channel.Received().BasicQos(0, 1, false);
  }

  [Fact]
  public void Constructor_StartsConsumingRegularMessages()
  {
    // assert
    _connectionManager.Received().CreateConsumer(Arg.Any<IModel>());
    _channel.Received().BasicConsume(_options.ClientQueue, false, _basicConsumer);
  }

  [Fact]
  public void Constructor_StartsConsumingFromReplyQueue()
  {
    // assert
    _connectionManager.Received().CreateConsumer(Arg.Any<IModel>());
    _channel.Received().BasicConsume("amq.rabbitmq.reply-to", false, _replyConsumer);
  }

  [Fact]
  public void Dispose_DisposesConnection()
  {
    // act
    _bus.Dispose();

    // assert
    _connectionManager.Received().Dispose();
  }

  [Fact]
  public void Constructor_Exception_StartsBasicConsumeOneMoreTime()
  {
    // arrange
    var exception = new Exception("something went wrong");
    var callbackExceptionEventArgs = new CallbackExceptionEventArgs(exception);

    // act
    _channel.CallbackException += Raise.EventWith(new object(), callbackExceptionEventArgs);

    // assert
    _channel.Received(1).Dispose();
    _connectionManager.Received().CreateConsumer(_channel);
  }

  [Fact]
  public void Register_AddsHandler()
  {
    // arrange
    _connectionManager.IsConnected.Returns(true);

    // act
    _bus.Register<ConcreteSecondHandler, ConcreteEvent>();

    // assert
    _channel.Received(1).QueueBind(_options.ClientQueue,
      _options.Exchange,
      nameof(ConcreteEvent));
  }

  [Fact]
  public void Register_ConnectionIsLost_Reconnects()
  {
    // arrange
    _connectionManager.IsConnected.Returns(false, true);

    // act
    _bus.Register<ConcreteSecondHandler, ConcreteEvent>();

    // assert
    _connectionManager.Received().TryConnect();
  }

  [Fact]
  public void Register_MultipleHandlersForSameEvent_AddsHandlers()
  {
    // arrange
    _bus.Register<ConcreteSecondHandler, ConcreteEvent>();
    _bus.Register<ConcreteThirdHandler, ConcreteEvent2>();

    // assert
    _channel.Received(1).QueueBind(_options.ClientQueue,
      _options.Exchange,
      Arg.Any<string>());
  }

  [Fact]
  public void Unregister_SingleHandler_RemovesHandler()
  {
    // arrange
    _bus.Register<ConcreteSecondHandler, ConcreteEvent>();

    // act
    _bus.Unregister<ConcreteSecondHandler, ConcreteEvent>();

    // assert
    _channel.Received(1).QueueUnbind(_options.ClientQueue,
      _options.Exchange,
      nameof(ConcreteEvent));
  }

  [Fact]
  public void Unregister_ConnectionIsLost_Reconnects()
  {
    // arrange
    _bus.Register<ConcreteSecondHandler, ConcreteEvent>();

    // act
    _bus.Unregister<ConcreteSecondHandler, ConcreteEvent>();

    // assert
    _connectionManager.Received(3).TryConnect();
  }

  [Fact]
  public void Unregister_NoHandler_ThrowNoSubscriptionFound()
  {
    // act
    var act = () => _bus.Unregister<ConcreteSecondHandler, ConcreteEvent>();

    // assert
    act.Should().Throw<NoSubscriptionFoundException>();
  }

  [Fact]
  public void Unregister_MultipleHandlersForSameEvent_RemovesSingleHandler()
  {
    // arrange
    _bus.Register<ConcreteSecondHandler, ConcreteEvent>();
    _bus.Register<ConcreteThirdHandler, ConcreteEvent2>();

    // act
    _bus.Unregister<ConcreteSecondHandler, ConcreteEvent>();

    // assert
    _channel.DidNotReceive().QueueUnbind(_options.ClientQueue,
      _options.Exchange,
      Arg.Any<string>());
  }

  [Fact]
  public void Publish_NotConnected_Connects()
  {
    // arrange
    var message = new ConcreteEvent(@"{""foo"": ""bar""}");
    _connectionManager.IsConnected.Returns(false, true);

    // act
    _bus.Publish(message);

    // assert
    _connectionManager.Received().TryConnect();
  }

  [Fact]
  public void Publish_Event_SendsMessage()
  {
    // act
    var message = new ConcreteEvent("foo");
    var json = JsonSerializer.Serialize(message,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
      });
    var body = Encoding.UTF8.GetBytes(json);
    var timestamp = new DateTimeOffset(message.CreatedAt).ToUnixTimeMilliseconds();

    // arrange
    _bus.Publish(message);

    // assert
    _channel.Received().BasicPublish(_options.Exchange,
      message.Type,
      true,
      Arg.Is<IBasicProperties>(x =>
        x.ContentType == "application/json" &&
        string.IsNullOrEmpty(x.ReplyTo) &&
        x.CorrelationId == message.CorrelationId &&
        x.Type == message.Type &&
        x.Persistent == true &&
        x.Timestamp.UnixTime == timestamp),
      Arg.Is<ReadOnlyMemory<byte>>(x => x.ToArray().SequenceEqual(body)));
  }

  [Fact]
  public async Task ReceiverHandler_RedeliveredEvent_SkipsMessage()
  {
    // act
    var eventHandler = Substitute.For<EventListener<ConcreteEvent>>();
    _scopeFactory.CreateScope().ServiceProvider.GetService(typeof(ConcreteSecondHandler)).Returns(eventHandler);
    _bus.Register<ConcreteSecondHandler, ConcreteEvent>();

    // arrange
    await _basicConsumer.HandleBasicDeliver(default, default, true, default, default, default, default);

    // assert
    await eventHandler.DidNotReceive().Handle(Arg.Any<ConcreteEvent>());
  }

  [Fact]
  public async Task Execute_GivenCommand_SendsCommandToQueue()
  {
    // arrange
    var command = new ConcreteCommand("foo", false);
    var timestamp = new DateTimeOffset(command.CreatedAt).ToUnixTimeMilliseconds();
    var json = JsonSerializer.Serialize(command,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
      });
    var body = Encoding.UTF8.GetBytes(json);

    // act
    var result = await _bus.Execute(command);

    // assert
    result.Should().BeOfType<Unit>();
    _channel.Received().BasicPublish("",
      _options.AppQueue,
      true,
      Arg.Is<IBasicProperties>(x =>
        x.ContentType == "application/json" &&
        x.ReplyTo == "amq.rabbitmq.reply-to" &&
        x.CorrelationId == command.CorrelationId &&
        x.Type == command.Type &&
        x.Persistent == true &&
        x.Timestamp.UnixTime == timestamp),
      Arg.Is<ReadOnlyMemory<byte>>(x => x.ToArray().SequenceEqual(body)));
  }

  [Fact]
  public async Task Execute_GivenCommand_SendsMessageToQueueAndGetsReply()
  {
    // arrange
    var command = new ConcreteCommand2("foo");
    var reply = new FooBar("bar");
    var json = JsonSerializer.Serialize(reply,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
      });
    var body = Encoding.UTF8.GetBytes(json);
    var basicProperties = Substitute.For<IBasicProperties>();
    basicProperties.Type = nameof(ConcreteCommand2);
    basicProperties.CorrelationId = command.CorrelationId;

    // act
    var task = _bus.Execute(command);
    await _replyConsumer.HandleBasicDeliver(default, default, false, default, default, basicProperties, body);
    var result = await task;

    // assert
    result.Should().BeOfType<FooBar>();
  }

  [Fact]
  public async Task Execute_NoReplyForGivenTime_ThrowsError()
  {
    // arrange
    var command = new ConcreteCommand2("foo", true, 1);
    var reply = new FooBar("bar");
    var json = JsonSerializer.Serialize(reply,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
      });
    var body = Encoding.UTF8.GetBytes(json);
    var basicProperties = Substitute.For<IBasicProperties>();
    basicProperties.Type = nameof(ConcreteCommand2);
    basicProperties.CorrelationId = command.CorrelationId;

    // act
    var act = () => _bus.Execute(command);

    // assert
    await act.Should().ThrowAsync<Exception>();
  }

  [Fact]
  public async Task ReceiverHandler_InstanceOfHandlerNotFound_SkipsMessage()
  {
    // arrange
    var message = new ConcreteEvent("foo");

    var json = JsonSerializer.Serialize(message,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
      });
    var body = Encoding.UTF8.GetBytes(json);
    var basicProperties = Substitute.For<IBasicProperties>();
    basicProperties.Type = nameof(ConcreteEvent);

    var eventHandler = Substitute.For<EventListener<ConcreteEvent>>();
    _bus.Register<ConcreteSecondHandler, ConcreteEvent>();

    // act
    await _basicConsumer.HandleBasicDeliver(default, default, false, default, default, basicProperties, body);

    // assert
    await eventHandler.DidNotReceive().Handle(Arg.Any<ConcreteEvent>());
  }

  [Fact]
  public async Task ReceiverHandler_SubscriptionNotFound_ThrowsError()
  {
    // arrange
    var message = new ConcreteEvent("foo");
    var json = JsonSerializer.Serialize(message,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
      });
    var body = Encoding.UTF8.GetBytes(json);
    var basicProperties = Substitute.For<IBasicProperties>();
    basicProperties.Type = nameof(ConcreteEvent);

    // act
    var act = () => _basicConsumer.HandleBasicDeliver(default, default, false, default, default, basicProperties, body);

    // assert
    await act.Should().ThrowAsync<NoSubscriptionFoundException>();
  }

  [Fact]
  public async Task ReceiverHandler_EventListenerThrowsError_SilentlyHandlesError()
  {
    // arrange
    var message = new ConcreteEvent("foo");

    var json = JsonSerializer.Serialize(message,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
      });
    var body = Encoding.UTF8.GetBytes(json);
    var basicProperties = Substitute.For<IBasicProperties>();
    basicProperties.Type = nameof(ConcreteEvent);

    var eventHandler = Substitute.For<EventListener<ConcreteEvent>>();
    var exception = new Exception("something went wrong");
    eventHandler.Handle(Arg.Any<ConcreteEvent>()).ThrowsAsync(exception);
    _scopeFactory.CreateScope().ServiceProvider.GetService(typeof(ConcreteSecondHandler)).Returns(eventHandler);
    _bus.Register<ConcreteSecondHandler, ConcreteEvent>();

    // act
    var act = () => _basicConsumer.HandleBasicDeliver(default, default, false, default, default, basicProperties, body);

    // assert
    await act.Should().NotThrowAsync();
  }

  [Fact]
  public async Task ReceiverHandler_NewEvent_RoutesMessageByType()
  {
    // arrange
    var message = new ConcreteEvent("foo");

    var json = JsonSerializer.Serialize(message,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
      });
    var body = Encoding.UTF8.GetBytes(json);

    var basicProperties = Substitute.For<IBasicProperties>();
    basicProperties.Type = nameof(ConcreteEvent);

    var eventHandler = Substitute.For<EventListener<ConcreteEvent>>();
    _scopeFactory.CreateScope().ServiceProvider.GetService(typeof(ConcreteSecondHandler)).Returns(eventHandler);
    _bus.Register<ConcreteSecondHandler, ConcreteEvent>();

    // act
    await _basicConsumer.HandleBasicDeliver(default, default, false, default, default, basicProperties, body);

    // assert
    await eventHandler.Received().Handle(Arg.Any<ConcreteEvent>());
  }

  [Fact]
  public async Task ReceiverHandler_NewEvent_RoutesMessageByRoutingKey()
  {
    // arrange
    var message = new ConcreteEvent("foo");

    var json = JsonSerializer.Serialize(message,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
      });
    var body = Encoding.UTF8.GetBytes(json);

    var basicProperties = Substitute.For<IBasicProperties>();

    var eventHandler = Substitute.For<EventListener<ConcreteEvent>>();
    _scopeFactory.CreateScope().ServiceProvider.GetService(typeof(ConcreteSecondHandler)).Returns(eventHandler);
    _bus.Register<ConcreteSecondHandler, ConcreteEvent>();

    // act
    await _basicConsumer.HandleBasicDeliver(default, default, false, default, nameof(ConcreteEvent), basicProperties, body);

    // assert
    await eventHandler.Received().Handle(Arg.Any<ConcreteEvent>());
  }

  [Fact]
  public async Task ReceiverHandler_Reply_SendsReply()
  {
    // arrange
    var message = new ConcreteEvent("foo");

    var json = JsonSerializer.Serialize(message,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
      });
    var body = Encoding.UTF8.GetBytes(json);

    var basicProperties = Substitute.For<IBasicProperties>();
    basicProperties.Type = nameof(ConcreteEvent);
    basicProperties.ReplyTo = "reply";
    basicProperties.CorrelationId = "1";

    var eventHandler = Substitute.For<EventListener<ConcreteEvent, FooBar>>();
    eventHandler.Handle(Arg.Any<ConcreteEvent>()).Returns(Task.FromResult(new FooBar("bar")));
    _scopeFactory.CreateScope().ServiceProvider.GetService(typeof(ConcreteFirstHandler)).Returns(eventHandler);
    _bus.Register<ConcreteFirstHandler, ConcreteEvent, FooBar>();

    // act
    await _basicConsumer.HandleBasicDeliver(default, default, false, default, default, basicProperties, body);

    // assert
    await eventHandler.Received().Handle(Arg.Any<ConcreteEvent>());
    _channel.Received().BasicPublish("",
      "reply",
      true,
      Arg.Is<IBasicProperties>(x =>
        x.ContentType == "application/json" &&
        x.CorrelationId == "1" &&
        x.Persistent == true),
      Arg.Any<ReadOnlyMemory<byte>>());

  }
}