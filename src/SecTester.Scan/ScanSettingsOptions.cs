using System;
using System.Collections.Generic;
using SecTester.Scan.Models;
using SecTester.Scan.Target;

namespace SecTester.Scan;

public interface ScanSettingsOptions
{
  /// <summary>
  ///   The list of tests to be performed against the target application
  /// </summary>
  IEnumerable<TestType> Tests { get; init; }

  /// <summary>
  ///   The target that will be attacked
  /// </summary>
  TargetOptions Target { get; init; }

  /// <summary>
  ///   The scan name
  /// </summary>
  string? Name { get; init; }

  /// <summary>
  ///   ID of the repeater
  /// </summary>
  string? RepeaterId { get; init; }

  /// <summary>
  ///   Determine whether scan is smart or simple
  /// </summary>
  bool? Smart { get; init; }

  /// <summary>
  ///   Pool size
  /// </summary>
  int? PoolSize { get; init; }

  /// <summary>
  ///   Threshold for slow entry points in milliseconds
  /// </summary>
  TimeSpan? SlowEpTimeout { get; init; }

  /// <summary>
  ///   Measure timeout responses from the target application globally,
  ///   and stop the scan if the target is unresponsive for longer than the specified time
  /// </summary>
  TimeSpan? TargetTimeout { get; init; }

  /// <summary>
  ///   Allows to skip testing static parameters.
  ///   ///
  /// </summary>
  bool? SkipStaticParams { get; init; }

  /// <summary>
  ///   Defines which part of the request to attack
  /// </summary>
  IEnumerable<AttackParamLocation>? AttackParamLocations { get; init; }
}
