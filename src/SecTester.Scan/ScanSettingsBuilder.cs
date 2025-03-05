using System;
using System.Collections.Generic;
using SecTester.Core.Utils;
using SecTester.Scan.Models;

namespace SecTester.Scan;

public class ScanSettingsBuilder
{
  private IEnumerable<AttackParamLocation> _attackParamLocations = new List<AttackParamLocation>
  {
    AttackParamLocation.Body, AttackParamLocation.Query, AttackParamLocation.Fragment
  };
  private string _name = "";
  private int _poolSize = 10;
  private string? _repeaterId;
  private bool _skipStaticParams = true;
  private TimeSpan _slowEpTimeout = TimeSpan.FromMilliseconds(1000);
  private bool _smart = true;
  private Target? _target;
  private TimeSpan _targetTimeout = TimeSpan.FromMinutes(5);
  private IEnumerable<string> _tests = new List<string>();

  /// <summary>
  ///   Sets a target for the scan.
  /// </summary>
  public ScanSettingsBuilder WithTarget(Target value)
  {
    _target = value ?? throw new ArgumentNullException(nameof(value));
    return this;
  }

  /// <summary>
  ///   Sets a name for the scan. If this method is not called, the scan will be given a default name based on the target URL
  ///   and HTTP method.
  /// </summary>
  public ScanSettingsBuilder WithName(string value)
  {
    _name = value ?? "";
    return this;
  }

  /// <summary>
  ///   Set a repeater to use for the scan.
  /// </summary>
  public ScanSettingsBuilder WithRepeater(string repeaterId)
  {
    _repeaterId = repeaterId;
    return this;
  }

  /// <summary>
  ///   Specifies whether the scan should use smart scanning. The enabled parameter is a boolean value that indicates whether
  ///   smart scanning should be enabled for the scan.
  /// </summary>
  public ScanSettingsBuilder Smart(bool enabled)
  {
    _smart = enabled;
    return this;
  }

  /// <summary>
  ///   Specifies whether the scan should skip static parameters. The skip parameter is a boolean value that indicates
  ///   whether static parameters should be skipped.
  /// </summary>
  public ScanSettingsBuilder SkipStaticParams(bool skip)
  {
    _skipStaticParams = skip;
    return this;
  }

  /// <summary>
  ///   Sets a size of the pool to use for the scan.
  /// </summary>
  public ScanSettingsBuilder WithPoolSize(int value)
  {
    _poolSize = value;

    return this;
  }

  /// <summary>
  ///   Set a timeout for slow endpoints. The value parameter is a TimeSpan object that represents the timeout for slow
  ///   endpoints.
  /// </summary>
  public ScanSettingsBuilder WithSlowEpTimeout(TimeSpan value)
  {
    _slowEpTimeout = value;

    return this;
  }

  /// <summary>
  ///   Sets a timeout for the target. The value parameter is a TimeSpan object that represents the timeout for the target.
  /// </summary>
  public ScanSettingsBuilder WithTargetTimeout(TimeSpan value)
  {
    _targetTimeout = value;

    return this;
  }

  /// <summary>
  ///   Sets a list of tests to run for the scan.
  /// </summary>
  public ScanSettingsBuilder WithTests(IEnumerable<string> value)
  {
    _tests = value;
    return this;
  }

  /// <summary>
  ///   Specifies locations of attack parameters.
  /// </summary>
  public ScanSettingsBuilder WithAttackParamLocations(IEnumerable<AttackParamLocation> value)
  {
    _attackParamLocations = value;
    return this;
  }

  /// <summary>
  ///   Once you have called the relevant methods to configure the settings for the scan, you can call this method to create
  ///   a ScanSettings instance.
  /// </summary>
  public ScanSettings Build()
  {
    if (_target is null)
    {
      throw new InvalidOperationException($"You have to provide a target by calling the {nameof(WithTarget)} method.");
    }

    var name = string.IsNullOrWhiteSpace(_name) ? CreateDefaultName(_target) : _name;

    return new ScanSettings(name, _target, _tests)
    {
      AttackParamLocations = _attackParamLocations,
      SlowEpTimeout = _slowEpTimeout,
      PoolSize = _poolSize,
      TargetTimeout = _targetTimeout,
      Smart = _smart,
      RepeaterId = _repeaterId,
      SkipStaticParams = _skipStaticParams
    };
  }

  private static string CreateDefaultName(Target target)
  {
    var uri = new Uri(target.Url);

    return $"{target.Method} {uri.Host}".Truncate(ScanSettings.MaxNameLength);
  }
}




