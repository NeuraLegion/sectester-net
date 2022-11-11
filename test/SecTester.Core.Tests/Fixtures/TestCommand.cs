namespace SecTester.Core.Tests.Fixtures;

internal record TestCommand(string Payload) : Command<string?>
{
  public string Payload = Payload;
}
