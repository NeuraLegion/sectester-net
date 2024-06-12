namespace SecTester.Repeater.Tests.Fixtures;

internal record ConcreteCommand : Command<Unit>
{
  public string Payload { get; }

  public ConcreteCommand(string payload, bool? expectReply = null, TimeSpan? ttl = null) : base(expectReply, ttl) => Payload = payload;
}
