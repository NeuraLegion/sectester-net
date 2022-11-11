namespace SecTester.Bus.Tests.Fixtures;

internal record ConcreteEvent(string Payload) : Event
{
  public string Payload = Payload;
}
