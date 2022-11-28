using SecTester.Repeater.Runners;

namespace SecTester.Repeater;

public record RepeaterOptions
{
  public string NamePrefix { get; init; } = "sectester";
  public string? Description { get; init; }
  public RequestRunnerOptions? RequestRunnerOptions { get; init; }
}
