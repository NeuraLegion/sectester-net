namespace SecTester.Scan.Tests.Fixtures;

public record TestScanSettingsOptions: ScanSettingsOptions
{
  public IEnumerable<TestType> Tests { get; init; }
  public TargetOptions Target { get; init; }
  public string? Name { get; init; }
  public string? RepeaterId { get; init; }
  public bool? Smart { get; init; }
  public int? PoolSize { get; init; }
  public TimeSpan? SlowEpTimeout { get; init; }
  public TimeSpan? TargetTimeout { get; init; }
  public bool? SkipStaticParams { get; init; }
  public IEnumerable<AttackParamLocation>? AttackParamLocations { get; init; }
}
