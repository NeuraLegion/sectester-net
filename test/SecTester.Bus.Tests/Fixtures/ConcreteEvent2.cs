namespace SecTester.Bus.Tests.Fixtures;

[MessageType(name: nameof(ConcreteEvent))]
internal record ConcreteEvent2(string Payload) : Event
{
  public string Payload = Payload;
}
