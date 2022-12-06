using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using SecTester.Core.Utils;
using SecTester.Scan.Models;
using SecTester.Scan.Target;

namespace SecTester.Scan;

public record ScanSettings : ScanSettingsOptions
{
  private const int MaxNameLength = 200;

  private readonly Target.Target _target;
  private readonly IEnumerable<TestType> _tests;
  private readonly string? _name;
  private readonly int? _poolSize = 10;
  private readonly TimeSpan? _targetTimeout = TimeSpan.FromSeconds(5);
  private readonly TimeSpan? _slowEpTimeout = TimeSpan.FromSeconds(1000);
  private readonly IEnumerable<AttackParamLocation>? _attackParamLocations = new List<AttackParamLocation>
  {
    AttackParamLocation.Body, AttackParamLocation.Query, AttackParamLocation.Fragment
  };

  public TargetOptions Target
  {
    get => _target;
    init
    {
      _target = new Target.Target(value);
    }
  }

  public string? Name
  {
    get => _name;
    init
    {
      if (string.IsNullOrWhiteSpace(value) || value!.Length > MaxNameLength)
      {
        throw new ArgumentException($"Name must be less than {MaxNameLength} characters.");
      }

      _name = value;
    }
  }

  public string? RepeaterId { get; init; }

  public bool? Smart { get; init; } = true;

  public bool? SkipStaticParams { get; init; } = true;

  public int? PoolSize
  {
    get => _poolSize;
    init
    {
      if (value is null or < 1 or > 50)
      {
        throw new ArgumentException("Invalid pool size.");
      }

      _poolSize = value;
    }
  }

  public TimeSpan? SlowEpTimeout
  {
    get => _slowEpTimeout;
    init
    {
      if (value is null || value < TimeSpan.FromSeconds(100))
      {
        throw new ArgumentException("Invalid slow entry point timeout.");
      }

      _slowEpTimeout = value;
    }
  }

  public TimeSpan? TargetTimeout
  {
    get => _targetTimeout;
    init
    {
      if (value is null || (value < TimeSpan.FromSeconds(1) || value > TimeSpan.FromSeconds(120)))
      {
        throw new ArgumentException("Invalid target connection timeout.");
      }

      _targetTimeout = value;
    }
  }

  public IEnumerable<TestType> Tests
  {
    get => _tests;
    init
    {
      if (value is null || !value.All(x => Enum.IsDefined(typeof(TestType), x)))
      {
        throw new ArgumentException("Unknown test type supplied.");
      }

      var unique = value.Distinct();

      if (!unique.Any())
      {
        throw new ArgumentException("Please provide at least one test.");
      }

      _tests = unique;
    }
  }

  public IEnumerable<AttackParamLocation>? AttackParamLocations
  {
    get => _attackParamLocations;
    init
    {
      if (value is null || !value.All(x => Enum.IsDefined(typeof(AttackParamLocation), x)))
      {
        throw new ArgumentException("Unknown attack param location supplied.");
      }

      var unique = value.Distinct();

      if (!unique.Any())
      {
        throw new ArgumentException("Please provide at least one attack parameter location.");
      }

      _attackParamLocations = unique;
    }
  }

  public ScanSettings(TargetOptions targetOptions, IEnumerable<TestType> tests)
  {
    Target = targetOptions;
    Tests = tests;
    Name = CreateDefaultName(_target!);
  }

  internal ScanSettings(ScanSettingsOptions scanSettingsOptions)
  {
    Target = scanSettingsOptions.Target;
    Tests = scanSettingsOptions.Tests;
    Name = string.IsNullOrWhiteSpace(scanSettingsOptions.Name) ? CreateDefaultName(_target!) : scanSettingsOptions.Name;
    RepeaterId = scanSettingsOptions.RepeaterId;
    Smart = scanSettingsOptions.Smart ?? Smart;
    SkipStaticParams = scanSettingsOptions.SkipStaticParams ?? SkipStaticParams;
    PoolSize = scanSettingsOptions.PoolSize ?? PoolSize;
    TargetTimeout = scanSettingsOptions.TargetTimeout ?? TargetTimeout;
    SlowEpTimeout = scanSettingsOptions.SlowEpTimeout ?? SlowEpTimeout;
    AttackParamLocations = scanSettingsOptions.AttackParamLocations ?? AttackParamLocations;
  }

  private static string CreateDefaultName(Target.Target target)
  {
    var uri = new Uri(target.Url);

    return $"{target.Method} {uri.Host}".Truncate(MaxNameLength - 1)!;
  }
}
