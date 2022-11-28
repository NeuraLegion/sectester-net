using System;

namespace SecTester.Scan;

public record ScanOptions(TimeSpan? Timeout = default, TimeSpan? PollingInterval = default)
{
  public TimeSpan? Timeout { get; set; } = Timeout;
  public TimeSpan? PollingInterval { get; set; } = PollingInterval;
}
