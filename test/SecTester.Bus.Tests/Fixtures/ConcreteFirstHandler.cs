namespace SecTester.Bus.Tests.Fixtures;

internal class ConcreteFirstHandler : IEventListener<ConcreteEvent, FooBar>
{
  public Task<FooBar> Handle(ConcreteEvent message)
  {
    return Task.FromResult(new FooBar("bar"));
  }
}
