namespace SecTester.Scan.Tests.Fixtures;

public record TestScanSettingsOptions(IEnumerable<TestType> Tests, TargetOptions Target) : ScanSettingsOptions
{
  public string? Name { get; init; }
  public string? RepeaterId { get; init; }
  public bool? Smart { get; init; }
  public int? PoolSize { get; init; }
  public TimeSpan? SlowEpTimeout { get; init; }
  public TimeSpan? TargetTimeout { get; init; }
  public bool? SkipStaticParams { get; init; }
  public IEnumerable<AttackParamLocation>? AttackParamLocations { get; init; }
}
