using System;
using System.Collections.Generic;

namespace SecTester.Scan.Models;

public record ScanConfig(string Name)
{
  public string Name { get; init; } = Name ?? throw new ArgumentNullException(nameof(Name));
  public Module? Module { get; init; }
  public IEnumerable<TestType>? Tests { get; init; }
  public IEnumerable<Discovery>? DiscoveryTypes { get; init; }
  public int? PoolSize { get; init; }
  public IEnumerable<AttackParamLocation>? AttackParamLocations { get; init; }
  public string? FileId { get; init; }
  public IEnumerable<string>? HostsFilter { get; init; }
  public IEnumerable<string>? Repeaters { get; init; }
  public bool? Smart { get; init; }
  public bool? SkipStaticParams { get; init; }
  public string? ProjectId { get; init; }
  public int? SlowEpTimeout { get; init; }
  public int? TargetTimeout { get; init; }
}
