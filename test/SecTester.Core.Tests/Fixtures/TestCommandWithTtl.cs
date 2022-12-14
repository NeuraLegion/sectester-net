namespace SecTester.Core.Tests.Fixtures;

internal record TestCommandWithTtl : TestCommand
{
  public TestCommandWithTtl(string payload, TimeSpan ttl) : base(payload)
  {
    Ttl = ttl;
  }
}
