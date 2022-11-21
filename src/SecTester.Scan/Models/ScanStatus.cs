using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum ScanStatus
{
  Failed,
  Disrupted,
  Running,
  Stopped,
  Queued,
  Scheduled,
  Pending,
  Done,
  Paused
}
