namespace SecTester.Bus.Dispatchers;

public interface IRmqEventBusFactory
{
  RmqEventBus CreateEventBus(RmqEventBusOptions options);
}
