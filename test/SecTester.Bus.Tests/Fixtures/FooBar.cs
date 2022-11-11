namespace SecTester.Bus.Tests.Fixtures;

internal record FooBar
{
  public string Foo { get; }

  [JsonConstructor]
  public FooBar(string foo)
  {
    Foo = foo;
  }
}
