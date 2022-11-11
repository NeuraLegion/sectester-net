namespace SecTester.Bus.Tests.Fixtures;

internal class ConcreteSecondHandler : EventListener<ConcreteEvent>
{
  public Task<Unit> Handle(ConcreteEvent message)
  {
    return Unit.Task;
  }
}
