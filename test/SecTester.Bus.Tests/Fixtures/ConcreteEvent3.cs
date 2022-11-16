namespace SecTester.Bus.Tests.Fixtures;

[MessageType(name: "custom")]
internal record ConcreteEvent3(string Payload) : Event
{
  public string Payload = Payload;
}
