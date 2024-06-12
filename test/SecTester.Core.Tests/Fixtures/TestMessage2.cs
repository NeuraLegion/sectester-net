namespace SecTester.Core.Tests.Fixtures;

[MessageType("custom")]
internal record TestMessage2 : Message
{
  public string Payload { get; }

  public TestMessage2(string payload)
  {
    Payload = payload;
  }

  public TestMessage2(string payload, string type, string correlationId, DateTime createdAt) : base(type, correlationId, createdAt)
  {
    Payload = payload;
  }
}
