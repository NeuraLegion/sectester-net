namespace SecTester.Core.Tests.Fixtures;

internal record TestMessage : Message
{
  public string Payload { get; }

  public TestMessage(string payload)
  {
    Payload = payload;
  }

  public TestMessage(string payload, string type, string correlationId, DateTime createdAt) : base(type, correlationId, createdAt)
  {
    Payload = payload;
  }
}
