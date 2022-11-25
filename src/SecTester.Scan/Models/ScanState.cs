using System;
using System.Collections.Generic;

namespace SecTester.Scan.Models;

public record ScanState(ScanStatus Status)
{
  public IEnumerable<IssueGroup>? IssuesBySeverity { get; init; }
  public int? EntryPoints { get; init; }
  public int? TotalParams { get; init; }
  public bool? Discovering { get; init; }
  public int? Requests { get; init; }
  public int? Elapsed { get; init; }
  public DateTime? StartTime { get; init; }
  public DateTime? EndTime { get; init; }
  public DateTime? CreatedAt { get; init; }
}
