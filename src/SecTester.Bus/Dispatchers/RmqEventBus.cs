using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecTester.Bus.Exceptions;
using SecTester.Bus.Extensions;
using SecTester.Core;
using SecTester.Core.Bus;
using SecTester.Core.Utils;

namespace SecTester.Bus.Dispatchers;

public class RmqEventBus : EventBus, IDisposable
{
  private readonly RmqConnectionManager _connectionManager;
  private readonly List<Type> _eventTypes = new();
  private readonly Dictionary<string, List<Type>> _handlers = new();
  private readonly ILogger _logger;
  private readonly RmqEventBusOptions _options;
  private readonly IServiceScopeFactory _scopeFactory;
  private IModel _channel;

  public RmqEventBus(RmqEventBusOptions options, RmqConnectionManager connectionManager, ILogger logger,
    IServiceScopeFactory scopeFactory)
  {
    _options = options ?? throw new ArgumentNullException(nameof(options));
    _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    _channel = CreateConsumerChannel();
  }

  public Task Publish<TEvent>(TEvent message) where TEvent : Event
  {
    _connectionManager.TryConnect();

    SendMessage(new MessageParams<TEvent>
    {
      Payload = message,
      Type = message.Type,
      RoutingKey = message.Type,
      Exchange = _options.Exchange,
      CorrelationId = message.CorrelationId,
      CreatedAt = message.CreatedAt
    });

    return Task.CompletedTask;
  }

  public Task<TResult?> Execute<TResult>(Command<TResult> message)
  {
    throw new NotImplementedException();
  }

  public void Register<THandler, TEvent, TResult>() where THandler : EventListener<TEvent, TResult> where TEvent : Event
  {
    var eventName = MessageUtils.GetMessageType<TEvent>();

    if (!_handlers.ContainsKey(eventName))
    {
      _eventTypes.Add(typeof(TEvent));
      _handlers.Add(eventName, new List<Type>());
      BindQueue(eventName);
    }

    _handlers[eventName].Add(typeof(THandler));
  }

  public void Unregister<THandler, TEvent, TResult>() where THandler : EventListener<TEvent, TResult> where TEvent : Event
  {
    var eventName = MessageUtils.GetMessageType<TEvent>();

    if (!_handlers.ContainsKey(eventName))
    {
      throw new NoSubscriptionFoundException(eventName);
    }

    _handlers[eventName].Remove(typeof(THandler));

    if (_handlers[eventName] is { Count: 0 })
    {
      _eventTypes.Remove(typeof(TEvent));
      _handlers.Remove(eventName);
      UnBindQueue(eventName);
    }
  }

  public void Register<THandler, TEvent>() where THandler : EventListener<TEvent> where TEvent : Event
  {
    Register<THandler, TEvent, Unit>();
  }

  public void Unregister<THandler, TEvent>() where THandler : EventListener<TEvent> where TEvent : Event
  {
    Unregister<THandler, TEvent, Unit>();
  }

  public void Dispose()
  {
    _connectionManager.Dispose();
    GC.SuppressFinalize(this);
  }

  private IModel CreateConsumerChannel()
  {
    _connectionManager.TryConnect();

    var channel = _connectionManager.CreateChannel();
    channel.CallbackException += (_, _) =>
    {
      _channel.Dispose();
      _channel = CreateConsumerChannel();
    };

    BindQueueToExchange(channel);
    StartBasicConsume(channel);

    return channel;
  }

  private void StartBasicConsume(IModel channel)
  {
    var consumer = _connectionManager.CreateConsumer(channel);
    consumer.Received += ReceiverHandler;
    channel.BasicConsume(_options.ClientQueue, false, consumer);
  }

  private void BindQueueToExchange(IModel channel)
  {
    channel.ExchangeDeclare(_options.Exchange, "direct", true);
    channel.QueueDeclare(_options.ClientQueue, exclusive: false, autoDelete: true, durable: true);
    channel.BasicQos(0, _options.PrefetchCount, false);
  }

  private async Task ReceiverHandler(object sender, BasicDeliverEventArgs args)
  {
    if (args.Redelivered)
    {
      return;
    }

    var name = string.IsNullOrEmpty(args.BasicProperties.Type) ? args.RoutingKey : args.BasicProperties.Type;
    var handlers = GetHandlers(name);
    var body = Encoding.UTF8.GetString(args.Body.ToArray());
    var consumedMessage = new ConsumedMessage
    {
      Name = name,
      Payload = body,
      ReplyTo = args.BasicProperties.ReplyTo,
      CorrelationId = args.BasicProperties.CorrelationId
    };

    _logger.LogDebug(
      "Received a event ({Name}) with following payload: {Body}", consumedMessage.Name,
      body
    );

    foreach (var handler in handlers)
    {
      await HandleEvent(handler, consumedMessage);
    }
  }

  private List<Type> GetHandlers(string eventName)
  {
    if (!_handlers.ContainsKey(eventName))
    {
      throw new NoSubscriptionFoundException(eventName);
    }

    if (_handlers[eventName] is null or { Count: 0 })
    {
      throw new EventHandlerNotFoundException(eventName);
    }

    return _handlers[eventName];
  }

  private async Task HandleEvent(Type eventHandler, ConsumedMessage consumedMessage)
  {
    try
    {
      await using var scope = _scopeFactory.CreateAsyncScope();
      var instance = scope.ServiceProvider.GetService(eventHandler);
      var eventType = GetEventType(consumedMessage.Name);

      if (instance == null || eventType == null)
      {
        return;
      }

      var concreteType = eventHandler.GetConcreteEventListenerType();
      var payload = DeserializePayload(consumedMessage.Payload, eventType);
      var method = concreteType.GetMethod("Handle");
      var task = (Task)method!.Invoke(instance, new[]
      {
        payload
      });

      var response = await task.Cast<object?>();

      if (response != null && !string.IsNullOrEmpty(consumedMessage.ReplyTo))
      {
        SendReplyOnEvent(consumedMessage, response);
      }
    }
    catch (Exception err)
    {
      _logger.LogDebug(err, "Error while processing a message ({CorrelationId}) due to error occurred. Event: {Payload}", consumedMessage.CorrelationId, consumedMessage.Payload);
    }
  }

  private void SendReplyOnEvent<T>(ConsumedMessage consumedMessage, T response)
  {
    _logger.LogDebug(
      "Sending a reply ({Event}) back with following payload: {Json}",
      consumedMessage.Name,
      response
    );

    SendMessage(new MessageParams<T>
    {
      Payload = response,
      RoutingKey = consumedMessage.ReplyTo!,
      CorrelationId = consumedMessage.CorrelationId
    });
  }

  private void SendMessage<T>(MessageParams<T> messageParams)
  {
    using var channel = _connectionManager.CreateChannel();
    var json = JsonSerializer.Serialize(messageParams.Payload,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
      });
    var body = Encoding.UTF8.GetBytes(json);
    var properties = channel.CreateBasicProperties();
    var timestamp = new DateTimeOffset(messageParams.CreatedAt ?? DateTime.UtcNow).ToUnixTimeMilliseconds();

    properties.CorrelationId = messageParams.CorrelationId;
    properties.Type = messageParams.Type;
    properties.Timestamp = new AmqpTimestamp(timestamp);
    properties.DeliveryMode = 2;
    properties.ContentType = "application/json";

    _logger.LogDebug("Send a message with following parameters: {Params}", messageParams);

    channel.BasicPublish(messageParams.Exchange ?? "",
      messageParams.RoutingKey,
      true,
      properties,
      body);
  }

  private static TPayload? DeserializePayload<TPayload>(string data)
  {
    return (TPayload?)DeserializePayload(data, typeof(TPayload));
  }

  private static object? DeserializePayload(string data, Type type)
  {
    return JsonSerializer.Deserialize(data, type,
      new JsonSerializerOptions
      {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      });
  }

  private void BindQueue(string eventName)
  {
    _connectionManager.TryConnect();
    using var channel = _connectionManager.CreateChannel();
    channel.QueueBind(_options.ClientQueue,
      _options.Exchange,
      eventName);
  }

  private void UnBindQueue(string eventName)
  {
    _connectionManager.TryConnect();
    using var channel = _connectionManager.CreateChannel();
    channel.QueueUnbind(_options.ClientQueue,
      _options.Exchange,
      eventName);
  }

  private Type? GetEventType(string eventName)
  {
    return _eventTypes.SingleOrDefault(x => MessageUtils.GetMessageType(x) == eventName);
  }
}
