using System;

namespace SecTester.Scan.Models;

public record ScanState(ScanStatus Status, IssueGroup[]? IssuesBySeverity = default, int? EntryPoints = default,
  int? TotalParams = default, bool? Discovering = default, int? Requests = default, int? Elapsed = default,
  DateTime? StartTime = default, DateTime? EndTime = default, DateTime? CreatedAt = default)
{
}
