namespace SecTester.Core.Tests.Fixtures;

internal record BazQux
{
  [JsonConstructor]
  public BazQux(string baz)
  {
    Baz = baz;
  }

  public string Baz { get; }
}
