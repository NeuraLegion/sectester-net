using System;

namespace SecTester.Scan.Models;

public class ScanConfig
{
  public string Name { get; set; }
  public Module? Module { get; set; }
  public TestType[]? Tests { get; set; }
  public Discovery[]? DiscoveryTypes { get; set; }
  public int? PoolSize { get; set; }
  public AttackParamLocation[]? AttackParamLocations { get; set; }
  public string? FileId { get; set; }
  public string[]? HostsFilter { get; set; }
  public string[]? Repeaters { get; set; }
  public bool? Smart { get; set; }
  public bool? SkipStaticParams { get; set; }
  public string? ProjectId { get; set; }
  public int? SlowEpTimeout { get; set; }
  public int? TargetTimeout { get; set; }

  public ScanConfig(string name, Module? module = default, TestType[]? tests = default,
    Discovery[]? discoveryTypes = default, int? poolSize = default,
    AttackParamLocation[]? attackParamLocations = default, string? fileId = default, string[]? hostsFilter = default,
    string[]? repeaters = default, bool? smart = default, bool? skipStaticParams = default, string? projectId = default,
    int? slowEpTimeout = default, int? targetTimeout = default)
  {
    Name = name ?? throw new ArgumentNullException(nameof(name));
    Module = module;
    Tests = tests;
    DiscoveryTypes = discoveryTypes;
    PoolSize = poolSize;
    AttackParamLocations = attackParamLocations;
    FileId = fileId;
    HostsFilter = hostsFilter;
    Repeaters = repeaters;
    Smart = smart;
    SkipStaticParams = skipStaticParams;
    ProjectId = projectId;
    SlowEpTimeout = slowEpTimeout;
    TargetTimeout = targetTimeout;
  }
}
