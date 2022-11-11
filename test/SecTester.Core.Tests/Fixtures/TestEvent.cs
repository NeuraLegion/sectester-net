namespace SecTester.Core.Tests.Fixtures;

internal record TestEvent(string Payload) : Event
{
  public string Payload = Payload;
}
