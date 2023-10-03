namespace SecTester.Repeater;

public record Runtime(string Version)
{
  public bool? ScriptsLoaded { get; init; }
  public string? Ci { get; init; }
  public string? Os { get; init; }
  public string? Arch { get; init; }
  public bool? Docker { get; init; }
  public string? Distribution { get; init; }
  public string? NetVersion { get; init; }
}
