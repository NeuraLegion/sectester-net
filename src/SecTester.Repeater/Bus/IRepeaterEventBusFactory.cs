using SecTester.Core.Bus;

namespace SecTester.Repeater.Bus;

public interface IRepeaterEventBusFactory
{
  IEventBus Create(string repeaterId);
}
