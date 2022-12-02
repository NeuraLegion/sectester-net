using System;
using SecTester.Bus.Dispatchers;
using SecTester.Core;
using SecTester.Core.Bus;

namespace SecTester.Repeater.Bus;

public class DefaultRepeaterEventBusFactory : RepeaterEventBusFactory
{
  private readonly Configuration _config;
  private readonly RmqEventBusFactory _rmqEventBusFactory;

  public DefaultRepeaterEventBusFactory(Configuration config, RmqEventBusFactory rmqEventBusFactory)
  {
    _config = config ?? throw new ArgumentNullException(nameof(config));
    _rmqEventBusFactory = rmqEventBusFactory ?? throw new ArgumentNullException(nameof(rmqEventBusFactory));
  }

  public EventBus Create(string repeaterId)
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
