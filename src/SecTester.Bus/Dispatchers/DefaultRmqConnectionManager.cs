using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecTester.Core.Bus;

namespace SecTester.Bus.Dispatchers;

public class DefaultRmqConnectionManager : RmqConnectionManager
{
  private readonly IConnectionFactory _connectionFactory;
  private readonly ILogger _logger;
  private readonly RetryStrategy _retryStrategy;
  private readonly object _sync = new();
  private IConnection? _connection;

  public DefaultRmqConnectionManager(IConnectionFactory connectionFactory, ILogger logger, RetryStrategy retryStrategy)
  {
    _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _retryStrategy = retryStrategy ?? throw new ArgumentNullException(nameof(retryStrategy));
  }

  public bool IsConnected => _connection is { IsOpen: true };

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

  public void TryConnect()
  {
    if (!IsConnected)
    {
      Connect();
    }
  }

  public AsyncEventingBasicConsumer CreateConsumer(IModel channel)
  {
    ThrowIfNotConnected();

    return new AsyncEventingBasicConsumer(channel);
  }

  public void Connect()
  {
    lock (_sync)
    {
      var task = _retryStrategy.Acquire(() =>
        Task.Run(() => _connectionFactory.CreateConnection())).ConfigureAwait(false);

      _connection = task.GetAwaiter().GetResult();

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

  public IModel CreateChannel()
  {
    ThrowIfNotConnected();

    return _connection!.CreateModel();
  }

  private void ThrowIfNotConnected()
  {
    if (!IsConnected)
    {
      throw new InvalidOperationException(
        "Please make sure that client established a connection with host.");
    }
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
