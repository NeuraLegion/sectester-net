using System;
using System.Collections.Generic;
using System.Linq;
using SecTester.Core.Utils;
using SecTester.Scan.Models;

namespace SecTester.Scan;

public record ScanSettings : ScanSettingsOptions
{
  internal const int MaxNameLength = 200;
  private const int MinPoolSize = 1;
  private const int MaxPoolSize = 50;

  private static readonly TimeSpan MinSlowEpTimeout = TimeSpan.FromSeconds(100);
  private static readonly TimeSpan MinTargetTimeout = TimeSpan.FromSeconds(1);
  private static readonly TimeSpan MaxTargetTimeout = TimeSpan.FromSeconds(120);

  private readonly IEnumerable<AttackParamLocation>? _attackParamLocations = new List<AttackParamLocation>
  {
    AttackParamLocation.Body, AttackParamLocation.Query, AttackParamLocation.Fragment
  };

  private readonly string _name;
  private readonly int? _poolSize = 10;
  private readonly TimeSpan? _slowEpTimeout = TimeSpan.FromSeconds(1000);

  private readonly Target _target;
  private readonly TimeSpan? _targetTimeout = TimeSpan.FromSeconds(5);
  private readonly IEnumerable<TestType> _tests;

#pragma warning disable CS8618
  public ScanSettings(TargetOptions targetOptions, IEnumerable<TestType> tests)
#pragma warning restore CS8618
  {
    Target = targetOptions;
    Tests = tests;
    Name = CreateDefaultName();
  }

#pragma warning disable CS8618
  internal ScanSettings(ScanSettingsOptions scanSettingsOptions)
#pragma warning restore CS8618
  {
    Target = scanSettingsOptions.Target;
    Tests = scanSettingsOptions.Tests;
    Name = string.IsNullOrWhiteSpace(scanSettingsOptions.Name) ? CreateDefaultName() : scanSettingsOptions.Name;
    RepeaterId = scanSettingsOptions.RepeaterId;
    Smart = scanSettingsOptions.Smart ?? Smart;
    SkipStaticParams = scanSettingsOptions.SkipStaticParams ?? SkipStaticParams;
    PoolSize = scanSettingsOptions.PoolSize ?? PoolSize;
    TargetTimeout = scanSettingsOptions.TargetTimeout ?? TargetTimeout;
    SlowEpTimeout = scanSettingsOptions.SlowEpTimeout ?? SlowEpTimeout;
    AttackParamLocations = scanSettingsOptions.AttackParamLocations ?? AttackParamLocations;
  }

  public TargetOptions Target
  {
    get => _target;
    init => _target = new Target(value ?? throw new ArgumentNullException(nameof(Target)));
  }

  public string Name
  {
    get => _name;
    init
    {
      if (string.IsNullOrWhiteSpace(value) || value.Length > MaxNameLength)
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
      if (value is null or < MinPoolSize or > MaxPoolSize)
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
      if (value is null || value < MinSlowEpTimeout)
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
      if (value is null || value < MinTargetTimeout || value > MaxTargetTimeout)
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
      if (value is null)
      {
        throw new ArgumentException("Tests must not be null.");
      }

      var validTests = Enum.GetValues(typeof(TestType)).Cast<TestType>().Except(value);

      if (validTests.Any())
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
      if (value is null)
      {
        throw new ArgumentException("Attack param locations must not be null.");
      }

      var validTests = Enum.GetValues(typeof(AttackParamLocation)).Cast<AttackParamLocation>().Except(value);

      if (validTests.Any())
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

  private string CreateDefaultName()
  {
    var uri = new Uri(Target.Url);

    return $"{Target.Method} {uri.Host}".Truncate(MaxNameLength);
  }
}
