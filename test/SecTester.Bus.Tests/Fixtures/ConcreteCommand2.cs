namespace SecTester.Bus.Tests.Fixtures;

internal record ConcreteCommand2 : Command<FooBar>
{
  public string Payload { get; }

  public ConcreteCommand2(string payload, bool? expectReply = null, int? ttl = null) : base(expectReply, ttl) => Payload = payload;
}
