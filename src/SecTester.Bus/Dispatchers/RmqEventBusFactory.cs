namespace SecTester.Bus.Dispatchers;

public interface RmqEventBusFactory
{
  RmqEventBus CreateEventBus(RmqEventBusOptions options);
}
