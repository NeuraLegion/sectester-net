using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecTester.Scan.Models;

namespace SecTester.Scan;

public class Scan : IDisposable
{
  private static readonly IReadOnlyCollection<ScanStatus> ActiveStatuses =
    new[] { ScanStatus.Pending, ScanStatus.Running, ScanStatus.Queued };

  private static readonly IReadOnlyCollection<ScanStatus> DoneStatuses =
    new[] { ScanStatus.Disrupted, ScanStatus.Done, ScanStatus.Failed, ScanStatus.Stopped };

  private readonly Scans _scans;
  private readonly int _pollingInterval;
  private readonly int _timeout;
  private ScanState _scanState;

  public bool Active => ActiveStatuses.Contains(_scanState.Status);

  public bool Done => DoneStatuses.Contains(_scanState.Status);

  public Scan(Scans scans, int pollingInterval, int timeout)
  {
    _scans = scans ?? throw new ArgumentNullException(nameof(scans));
    _pollingInterval = pollingInterval;
    _timeout = timeout;
    _scanState = new ScanState(ScanStatus.Pending);
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }

  public Task<Issue[]> Issues(int? limit, string? nextId, DateTime? nextCreatedAt)
  {
    throw new NotImplementedException();
  }

  public Task Stop()
  {
    throw new NotImplementedException();
  }

  public IAsyncEnumerable<ScanState> Status()
  {
    throw new NotImplementedException();
  }
}
