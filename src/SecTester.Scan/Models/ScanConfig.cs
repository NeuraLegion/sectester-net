using System;

namespace SecTester.Scan.Models;

public record ScanConfig(string Name, Module? Module = default, TestType[]? Tests = default,
  Discovery[]? DiscoveryTypes = default, int? PoolSize = default,
  AttackParamLocation[]? AttackParamLocations = default, string? FileId = default, string[]? HostsFilter = default,
  string[]? Repeaters = default, bool? Smart = default, bool? SkipStaticParams = default, string? ProjectId = default,
  int? SlowEpTimeout = default, int? TargetTimeout = default)
{
  public string Name { get; init; } = Name ?? throw new ArgumentNullException(nameof(Name));
}
