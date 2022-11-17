using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Scan.Models;

namespace SecTester.Scan;

public class Scan : IDisposable
{
  private static readonly IReadOnlyCollection<ScanStatus> ActiveStatuses =
    new[]
    {
      ScanStatus.Pending, ScanStatus.Running, ScanStatus.Queued
    };

  private static readonly IReadOnlyCollection<ScanStatus> DoneStatuses =
    new[]
    {
      ScanStatus.Disrupted, ScanStatus.Done, ScanStatus.Failed, ScanStatus.Stopped
    };

  private readonly ScanOptions _options;

  private readonly Scans _scans;
  private readonly ScanState _scanState = new(ScanStatus.Pending);

  public Scan(Scans scans, ScanOptions options)
  {
    _scans = scans ?? throw new ArgumentNullException(nameof(scans));
    _options = options ?? throw new ArgumentNullException(nameof(options));
  }

  public bool Active => ActiveStatuses.Contains(_scanState.Status);

  public bool Done => DoneStatuses.Contains(_scanState.Status);

  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }

  public Task<IEnumerable<Issue>> Issues(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task Stop()
  {
    throw new NotImplementedException();
  }

  public IAsyncEnumerable<ScanState> Status(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
