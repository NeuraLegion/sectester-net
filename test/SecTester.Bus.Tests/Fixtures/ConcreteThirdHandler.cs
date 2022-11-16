namespace SecTester.Bus.Tests.Fixtures;

internal class ConcreteThirdHandler : EventListener<ConcreteEvent2>
{
  public Task<Unit> Handle(ConcreteEvent2 message)
  {
    return Unit.Task;
  }
}
