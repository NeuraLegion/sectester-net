namespace SecTester.Core.Tests.Fixtures;

internal record TestCommandWithTtl : TestCommand
{
  public TestCommandWithTtl(string payload, int ttl) : base(payload)
  {
    Ttl = ttl;
  }
}
