namespace SecTester.Scan.Target.Har;

public record PostDataParameter(string Name, string Value) : Parameter(Name, Value)
{
  public string? FileName { get; init; }
  public string? ContentType { get; init; }
}
