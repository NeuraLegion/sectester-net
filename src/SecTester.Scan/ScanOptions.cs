using System;

namespace SecTester.Scan;

public record ScanOptions
{
  public ScanOptions(TimeSpan? timeout = default, TimeSpan? pollingInterval = default, bool deleteOnDispose = false)
  {
    Timeout = timeout ?? Timeout;
    PollingInterval = pollingInterval ?? PollingInterval;
    DeleteOnDispose = deleteOnDispose;
  }

  public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(300);

  public TimeSpan PollingInterval { get; init; } = TimeSpan.FromSeconds(5);

  public bool DeleteOnDispose { get; init; }
}
