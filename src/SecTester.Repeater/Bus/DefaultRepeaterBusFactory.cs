using System;
using Microsoft.Extensions.Logging;
using SecTester.Core;
using SocketIO.Serializer.MessagePack;
using SocketIOClient;
using SocketIOClient.Transport;

namespace SecTester.Repeater.Bus;

public class DefaultRepeaterBusFactory : IRepeaterBusFactory
{
  private readonly Configuration _config;
  private readonly ILoggerFactory _loggerFactory;

  public DefaultRepeaterBusFactory(Configuration config, ILoggerFactory loggerFactory)
  {
    _config = config ?? throw new ArgumentNullException(nameof(config));
    _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
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
      Transport = TransportProtocol.WebSocket,
      Auth = new { token = _config.Credentials.Token, domain = repeaterId }
    })
    {
      Serializer = new SocketIOMessagePackSerializer()
    };
    var wrapper = new SocketIoConnection(client);

    return new SocketIoRepeaterBus(options, wrapper, _loggerFactory.CreateLogger<IRepeaterBus>());
  }
}
