namespace SecTester.Bus.Tests.Fixtures;

internal record ConcreteCommand : Command<Unit>
{
  public string Payload { get; }

  public ConcreteCommand(string payload, bool? expectReply = null, int? ttl = null) : base(expectReply, ttl) => Payload = payload;
}
