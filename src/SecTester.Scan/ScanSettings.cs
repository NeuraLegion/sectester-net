using System;
using System.Collections.Generic;
using System.Linq;
using SecTester.Scan.Models;

namespace SecTester.Scan;

public sealed record ScanSettings
{
  internal const int MaxNameLength = 200;
  private const int MinPoolSize = 1;
  private const int MaxPoolSize = 50;

  private static readonly TimeSpan MinSlowEpTimeout = TimeSpan.FromSeconds(100);
  private static readonly TimeSpan MinTargetTimeout = TimeSpan.FromSeconds(1);
  private static readonly TimeSpan MaxTargetTimeout = TimeSpan.FromSeconds(120);

  private readonly IEnumerable<AttackParamLocation>? _attackParamLocations;
  private readonly string _name;
  private readonly int? _poolSize;
  private readonly TimeSpan? _slowEpTimeout;
  private readonly TimeSpan? _targetTimeout;
  private readonly IEnumerable<TestType> _tests;
  private readonly TargetOptions _target;

  public ScanSettings(string name, TargetOptions target, IEnumerable<TestType> tests)
  {
    Name = name;
    Target = target;
    Tests = tests;
  }

  /// <summary>
  ///   The target that will be attacked
  /// </summary>
  public TargetOptions Target
  {
    get => _target;
    init => _target = new Target(value ?? throw new ArgumentNullException(nameof(Target)));
  }

  /// <summary>
  ///   The scan name
  /// </summary>
  public string Name
  {
    get => _name;
    init
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException($"{nameof(Name)} cannot be null or empty");
      }

      if (value.Length > MaxNameLength)
      {
        throw new ArgumentException($"{nameof(Name)} must be less than {MaxNameLength} characters.");
      }

      _name = value;
    }
  }

  /// <summary>
  ///   ID of the repeater
  /// </summary>
  public string? RepeaterId { get; init; }

  /// <summary>
  ///   Determine whether scan is smart or simple
  /// </summary>
  public bool? Smart { get; init; }

  /// <summary>
  ///   Allows to skip testing static parameters.
  ///   ///
  /// </summary>
  public bool? SkipStaticParams { get; init; }

  /// <summary>
  ///   Pool size
  /// </summary>
  public int? PoolSize
  {
    get => _poolSize;
    init
    {
      if (value is < MinPoolSize or > MaxPoolSize)
      {
        throw new ArgumentException("Invalid pool size.");
      }

      _poolSize = value;
    }
  }

  /// <summary>
  ///   Threshold for slow entry points in milliseconds
  /// </summary>
  public TimeSpan? SlowEpTimeout
  {
    get => _slowEpTimeout;
    init
    {
      if (value < MinSlowEpTimeout)
      {
        throw new ArgumentException("Invalid slow entry point timeout.");
      }

      _slowEpTimeout = value;
    }
  }

  /// <summary>
  ///   Measure timeout responses from the target application globally,
  ///   and stop the scan if the target is unresponsive for longer than the specified time
  /// </summary>
  public TimeSpan? TargetTimeout
  {
    get => _targetTimeout;
    init
    {
      if (value < MinTargetTimeout || value > MaxTargetTimeout)
      {
        throw new ArgumentException("Invalid target connection timeout.");
      }

      _targetTimeout = value;
    }
  }

  /// <summary>
  ///   The list of tests to be performed against the target application
  /// </summary>
  public IEnumerable<TestType> Tests
  {
    get => _tests;
    init
    {
      if (value is null)
      {
        throw new ArgumentNullException(nameof(Tests));
      }

      if (value.Any(x => !Enum.IsDefined(typeof(TestType), x)))
      {
        throw new ArgumentException("Unknown test type supplied.");
      }

      var unique = value.Distinct().ToArray();

      if (!unique.Any())
      {
        throw new ArgumentException("Please provide at least one test.");
      }

      _tests = unique;
    }
  }

  /// <summary>
  ///   Defines which part of the request to attack
  /// </summary>
  public IEnumerable<AttackParamLocation>? AttackParamLocations
  {
    get => _attackParamLocations;
    init
    {
      if (value is null)
      {
        return;
      }

      if (value.Any(x => !Enum.IsDefined(typeof(AttackParamLocation), x)))
      {
        throw new ArgumentException("Unknown attack param location supplied.");
      }

      var unique = value.Distinct().ToArray();

      if (!unique.Any())
      {
        throw new ArgumentException("Please provide at least one attack parameter location.");
      }

      _attackParamLocations = unique;
    }
  }
}

