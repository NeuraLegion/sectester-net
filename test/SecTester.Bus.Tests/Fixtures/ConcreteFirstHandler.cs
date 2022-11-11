namespace SecTester.Bus.Tests.Fixtures;

internal class ConcreteFirstHandler : EventListener<ConcreteEvent, FooBar>
{
  public Task<FooBar> Handle(ConcreteEvent message)
  {
    return Task.FromResult(new FooBar("bar"));
  }
}
