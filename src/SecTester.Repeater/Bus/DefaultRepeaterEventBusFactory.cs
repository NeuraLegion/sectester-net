using System;
using SecTester.Bus.Dispatchers;
using SecTester.Core;
using SecTester.Core.Bus;

namespace SecTester.Repeater.Bus;

public class DefaultRepeaterEventBusFactory : IRepeaterEventBusFactory
{
  private readonly Configuration _config;
  private readonly IRmqEventBusFactory _rmqEventBusFactory;

  public DefaultRepeaterEventBusFactory(Configuration config, IRmqEventBusFactory rmqEventBusFactory)
  {
    _config = config ?? throw new ArgumentNullException(nameof(config));
    _rmqEventBusFactory = rmqEventBusFactory ?? throw new ArgumentNullException(nameof(rmqEventBusFactory));
  }

  public IEventBus Create(string repeaterId)
  {
    if (_config.Credentials == null)
    {
      throw new InvalidOperationException(
        "Please provide credentials to establish a connection with the bus."
      );
    }

    var options = new RmqEventBusOptions(_config.Bus, "app", "EventBus", $"agent:{repeaterId}")
    {
      Username = "bot",
      Password = _config.Credentials!.Token,
      HeartbeatInterval = TimeSpan.FromSeconds(30),
      ConnectTimeout = TimeSpan.FromSeconds(30)
    };

    return _rmqEventBusFactory.CreateEventBus(options);
  }
}
