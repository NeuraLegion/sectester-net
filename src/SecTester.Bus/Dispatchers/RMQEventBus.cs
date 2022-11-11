using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecTester.Core.Bus;

namespace SecTester.Bus.Dispatchers;

public class RMQEventBus : EventBus, IDisposable
{
  private readonly IConnectionFactory _connectionFactory;
  private readonly ILogger _logger;
  private IConnection? _connection;
  private readonly object _sync = new();
  private readonly RMQEventBusOptions _options;

  public RMQEventBus(RMQEventBusOptions options, IConnectionFactory connectionFactory, ILogger logger)
  {
    _options = options ?? throw new ArgumentNullException(nameof(options));
    _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public void Dispose()
  {
    if (!IsConnected)
    {
      return;
    }

    _connection.ConnectionShutdown -= ConnectionShutdown;
    _connection.CallbackException -= CallbackException;
    _connection.ConnectionBlocked -= ConnectionBlocked;
    _connection.Close();
    _connection.Dispose();
    GC.SuppressFinalize(this);
  }

  private bool IsConnected => _connection != null && _connection is { IsOpen: true };

  public void Connect()
  {
    lock (_sync)
    {
      _connection = _connectionFactory.CreateConnection();

      if (!IsConnected)
      {
        return;
      }

      _connection.ConnectionShutdown += ConnectionShutdown;
      _connection.CallbackException += CallbackException;
      _connection.ConnectionBlocked += ConnectionBlocked;
      _logger.LogDebug("Event bus connected to {Hostname}", _connection.Endpoint.HostName);
    }
  }

  public Task Publish(Event @event)
  {
    if (!IsConnected)
    {
      throw new InvalidOperationException(
        $"Please make sure that {nameof(EventBus)} established a connection with host.");
    }

    _logger.LogDebug("Send a message with following parameters: {Event}", @event);

    var type = @event.Type;
    using var channel = _connection.CreateModel();
    channel.ExchangeDeclare(exchange: _options.Exchange,
      type: "direct");

    string message = JsonSerializer.Serialize(@event, new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      IncludeFields = true
    });
    byte[] body = Encoding.UTF8.GetBytes(message);

    IBasicProperties properties = channel.CreateBasicProperties();
    properties.CorrelationId = @event.CorrelationId;
    properties.Type = type;
    properties.Timestamp = new AmqpTimestamp(@event.CreatedAt.Millisecond);
    properties.DeliveryMode = 2;

    channel.BasicPublish(exchange: _options.Exchange,
      routingKey: type,
      mandatory: true,
      basicProperties: properties,
      body: body);

    return Task.CompletedTask;
  }

  public Task<TResult?> Execute<TResult>(Command<TResult> message)
  {
    throw new System.NotImplementedException();
  }

  public void Register<THandler, TEvent>() where THandler : Core.Bus.EventHandler<TEvent> where TEvent : Event
  {
    throw new System.NotImplementedException();
  }

  public void Unregister<THandler, TEvent>() where THandler : Core.Bus.EventHandler<TEvent> where TEvent : Event
  {
    throw new System.NotImplementedException();
  }

  private void ConnectionBlocked(object sender, ConnectionBlockedEventArgs args)
  {
    _logger.LogWarning("A Event Bus connection blocked. Trying to re-connect.");
    Connect();
  }

  private void ConnectionShutdown(object sender, ShutdownEventArgs args)
  {
    _logger.LogWarning("A Event Bus connection shutdown. Trying to re-connect.");
    Connect();
  }

  private void CallbackException(object sender, CallbackExceptionEventArgs args)
  {
    _logger.LogWarning("A Event Bus connection throw exception. Trying to re-connect.");
    Connect();
  }
}
