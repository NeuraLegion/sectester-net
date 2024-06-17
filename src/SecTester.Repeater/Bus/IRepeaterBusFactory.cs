namespace SecTester.Repeater.Bus;

public interface IRepeaterBusFactory
{
  IRepeaterBus Create(string? namePrefix = default);
}
