using System;

namespace SecTester.Scan.Models;

public class ScanState
{
  public ScanStatus Status { get; set; }
  public IssueGroup[]? IssuesBySeverity { get; set; }
  public int? EntryPoints { get; set; }
  public int? TotalParams { get; set; }
  public bool? Discovering { get; set; }
  public int? Requests { get; set; }
  public int? Elapsed { get; set; }
  public DateTime? StartTime { get; set; }
  public DateTime? EndTime { get; set; }
  public DateTime? CreatedAt { get; set; }

  public ScanState(ScanStatus status, IssueGroup[]? issuesBySeverity = default, int? entryPoints = default,
    int? totalParams = default, bool? discovering = default, int? requests = default, int? elapsed = default,
    DateTime? startTime = default, DateTime? endTime = default, DateTime? createdAt = default)
  {
    Status = status;
    IssuesBySeverity = issuesBySeverity;
    EntryPoints = entryPoints;
    TotalParams = totalParams;
    Discovering = discovering;
    Requests = requests;
    Elapsed = elapsed;
    StartTime = startTime;
    EndTime = endTime;
    CreatedAt = createdAt;
  }
}
