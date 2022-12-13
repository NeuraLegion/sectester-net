using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SecTester.Core.Bus;

namespace SecTester.Bus.Dispatchers;

[ExcludeFromCodeCoverage]
public class DefaultRmqEventBusFactory : RmqEventBusFactory
{
  private readonly RetryStrategy _retryStrategy;
  private readonly IServiceScopeFactory _serviceScopeFactory;
  private readonly ILoggerFactory _loggerFactory;

  public DefaultRmqEventBusFactory(IServiceScopeFactory serviceScopeFactory, RetryStrategy retryStrategy,
    ILoggerFactory loggerFactory)
  {
    _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    _retryStrategy = retryStrategy ?? throw new ArgumentNullException(nameof(retryStrategy));
    _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
  }

  public RmqEventBus CreateEventBus(RmqEventBusOptions options)
  {
    var connectionManager = CreateConnectionManager(options);
    return new RmqEventBus(options, connectionManager, _loggerFactory.CreateLogger<RmqEventBus>(), _serviceScopeFactory);
  }

  protected virtual RmqConnectionManager CreateConnectionManager(RmqEventBusOptions options)
  {
    var factory = new ConnectionFactory
    {
      Uri = new Uri(options.Url),
      RequestedHeartbeat = options.HeartbeatInterval,
      RequestedConnectionTimeout = options.ConnectTimeout,
      DispatchConsumersAsync = true,
      Password = options.Password,
      UserName = options.Username
    };

    return new DefaultRmqConnectionManager(factory, _loggerFactory.CreateLogger<DefaultRmqConnectionManager>(), _retryStrategy);
  }
}
