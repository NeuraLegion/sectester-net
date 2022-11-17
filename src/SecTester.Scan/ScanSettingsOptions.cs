using System;
using System.Collections.Generic;
using SecTester.Scan.Models;
using SecTester.Scan.Target;

namespace SecTester.Scan;

public record ScanSettingsOptions(IEnumerable<TestType> Tests, TargetOptions Target, string? Name, string? RepeaterId, bool? Smart,
  int? PoolSize, int? SlowEpTimeout, int? TargetTimeout, bool? SkipStaticParams,
  IEnumerable<AttackParamLocation>? AttackParamLocations)
{
  public IEnumerable<TestType> Tests { get; init; } = Tests ?? throw new ArgumentNullException(nameof(Tests)); 
  public TargetOptions Target { get; init; } = Target ?? throw new ArgumentNullException(nameof(Target));
}
