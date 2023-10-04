using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecTester.Core;
using SecTester.Core.Utils;
using SocketIO.Serializer.MessagePack;
using SocketIOClient;

namespace SecTester.Repeater.Bus;

public class DefaultRepeaterBusFactory : IRepeaterBusFactory
{
  private readonly Configuration _config;
  private readonly ILoggerFactory _loggerFactory;
  private readonly IServiceScopeFactory _scopeFactory;

  public DefaultRepeaterBusFactory(Configuration config, ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory)
  {
    _config = config ?? throw new ArgumentNullException(nameof(config));
    _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
  }

  public IRepeaterBus Create(string repeaterId)
  {
    if (_config.Credentials is null)
    {
      throw new InvalidOperationException(
        "Please provide credentials to establish a connection with the bus."
      );
    }

    var options = new SocketIoRepeaterBusOptions(new Uri(_config.Api));
    var client = new SocketIOClient.SocketIO(options.Url, new SocketIOOptions
    {
      Path = options.Path,
      ReconnectionAttempts = options.ReconnectionAttempts,
      ReconnectionDelayMax = options.ReconnectionDelayMax,
      ConnectionTimeout = options.ConnectionTimeout,
      AutoUpgrade = false,
      Auth = new { token = _config.Credentials.Token, domain = repeaterId }
    })
    {
      Serializer = new SocketIOMessagePackSerializer()
    };
    var wrapper = new SocketIoConnection(client);

    var scope = _scopeFactory.CreateAsyncScope();
    var timerProvider = scope.ServiceProvider.GetRequiredService<ITimerProvider>();

    return new SocketIoRepeaterBus(options, wrapper, timerProvider, _loggerFactory.CreateLogger<IRepeaterBus>());
  }
}
