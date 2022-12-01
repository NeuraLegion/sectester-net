using SecTester.Core.Bus;

namespace SecTester.Repeater.Bus;

public interface RepeaterEventBusFactory
{
  EventBus Create(string repeaterId);
}
