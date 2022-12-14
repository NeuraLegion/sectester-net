namespace SecTester.Bus.Tests.Fixtures;

internal class ConcreteSecondHandler : IEventListener<ConcreteEvent>
{
  public Task<Unit> Handle(ConcreteEvent message)
  {
    return Unit.Task;
  }
}
