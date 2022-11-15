namespace SecTester.Scan;

public class ScanOptions
{
  public int? Timeout { get; set; }
  public int? PollingInterval { get; set; }

  public ScanOptions(int? timeout = default, int? pollingInterval = default)
  {
    Timeout = timeout;
    PollingInterval = pollingInterval;
  }
}
