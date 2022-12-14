namespace SecTester.Scan.Models.HarSpec;

public record PostDataParameter(string Name, string Value) : Parameter(Name, Value)
{
  public string? FileName { get; init; }
  public string? ContentType { get; init; }
}
