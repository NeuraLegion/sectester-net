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
  private TimeSpan _slowEpTimeout = TimeSpan.FromSeconds(1000);
  private bool _smart = true;
  private TargetOptions? _target;
  private TimeSpan _targetTimeout = TimeSpan.FromSeconds(5);
  private IEnumerable<TestType> _tests = new List<TestType>();

  public ScanSettingsBuilder WithTarget(TargetOptions value)
  {
    _target = new Target(value ?? throw new ArgumentNullException(nameof(value)));
    return this;
  }

  public ScanSettingsBuilder WithName(string value)
  {
    _name = value ?? "";
    return this;
  }

  public ScanSettingsBuilder WithRepeater(string repeaterId)
  {
    _repeaterId = repeaterId;
    return this;
  }

  public ScanSettingsBuilder Smart(bool enabled)
  {
    _smart = enabled;
    return this;
  }

  public ScanSettingsBuilder SkipStaticParams(bool skip)
  {
    _skipStaticParams = skip;
    return this;
  }

  public ScanSettingsBuilder WithPoolSize(int value)
  {
    _poolSize = value;

    return this;
  }

  public ScanSettingsBuilder WithSlowEpTimeout(TimeSpan value)
  {
    _slowEpTimeout = value;

    return this;
  }

  public ScanSettingsBuilder WithTargetTimeout(TimeSpan value)
  {
    _targetTimeout = value;

    return this;
  }

  public ScanSettingsBuilder WithTests(IEnumerable<TestType> value)
  {
    _tests = value;
    return this;
  }

  public ScanSettingsBuilder WithAttackParamLocations(IEnumerable<AttackParamLocation> value)
  {
    _attackParamLocations = value;
    return this;
  }

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

  private static string CreateDefaultName(TargetOptions target)
  {
    var uri = new Uri(target.Url);

    return $"{target.Method} {uri.Host}".Truncate(ScanSettings.MaxNameLength);
  }
}



