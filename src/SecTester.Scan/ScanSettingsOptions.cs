using SecTester.Scan.Models;
using SecTester.Scan.Target;

namespace SecTester.Scan;

public interface ScanSettingsOptions
{
  /// <summary>
  /// The list of tests to be performed against the target application 
  /// </summary>
  TestType[] Tests { get; set; }

  /// <summary>
  /// The target that will be attacked
  /// </summary>
  TargetOptions Target { get; set; }

  /// <summary>
  /// The scan name 
  /// </summary>
  string? Name { get; set; }

  /// <summary>
  /// ID of the repeater
  /// </summary>
  string? RepeaterId { get; set; }

  /// <summary>
  /// Determine whether scan is smart or simple
  /// </summary>
  bool? Smart { get; set; }

  /// <summary>
  /// Pool size
  /// </summary>
  int? PoolSize { get; set; }

  /// <summary>
  /// Threshold for slow entry points in milliseconds
  /// </summary>
  int? SlowEpTimeout { get; set; }

  /// <summary>
  /// Measure timeout responses from the target application globally,
  /// and stop the scan if the target is unresponsive for longer than the specified time
  /// </summary> 
  int? TargetTimeout { get; set; }

  /// <summary>
  /// Allows to skip testing static parameters.
  /// /// </summary>
  bool? SkipStaticParams { get; set; }

  /// <summary>
  /// Defines which part of the request to attack
  /// </summary>
  AttackParamLocation[]? AttackParamLocations { get; set; }
}
