using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SecTester.Core.Extensions;
using SecTester.Scan.Models;

namespace SecTester.Scan;

public class Scan : IAsyncDisposable
{
  private static readonly IReadOnlyCollection<ScanStatus> ActiveStatuses =
    new[] { ScanStatus.Pending, ScanStatus.Running, ScanStatus.Queued };

  private static readonly IReadOnlyCollection<ScanStatus> DoneStatuses =
    new[] { ScanStatus.Disrupted, ScanStatus.Done, ScanStatus.Failed, ScanStatus.Stopped };

  private readonly ScanOptions _options;
  private readonly ILogger _logger;
  private readonly Scans _scans;
  private ScanState _scanState = new(ScanStatus.Pending);
  private readonly SemaphoreSlim _semaphore = new(1, 1);

  public string Id { get; }

  public bool Active => ActiveStatuses.Contains(_scanState.Status);

  public bool Done => DoneStatuses.Contains(_scanState.Status);

  public Scan(string id, Scans scans, ILogger logger, ScanOptions options)
  {
    Id = id ?? throw new ArgumentNullException(nameof(id));
    _scans = scans ?? throw new ArgumentNullException(nameof(scans));
    _options = options ?? throw new ArgumentNullException(nameof(options));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public async ValueTask DisposeAsync()
  {
    try
    {
      await DisposeAsyncCore().ConfigureAwait(false);
    }
    catch
    {
      // ignore
    }

    _semaphore.Dispose();

    GC.SuppressFinalize(this);
  }

  protected virtual async ValueTask DisposeAsyncCore()
  {
    if (_options.DeleteOnDispose is true)
    {
      await _scans.DeleteScan(Id).ConfigureAwait(false);
    }
  }

  public Task<IEnumerable<Issue>> Issues()
  {
    return _scans.ListIssues(Id);
  }

  public async Task Stop()
  {
    try
    {
      await RefreshState().ConfigureAwait(false);

      if (Active)
      {
        await _scans.StopScan(Id).ConfigureAwait(false);
      }
    }
    catch
    {
      // ignore
    }
  }

  public async IAsyncEnumerable<ScanState> Status(
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    while (Active)
    {
      await Task.Delay(_options.PollingInterval, cancellationToken).ConfigureAwait(false);
      yield return await RefreshState(cancellationToken).ConfigureAwait(false);
    }

    yield return _scanState;
  }

  private async Task<ScanState> RefreshState(CancellationToken cancellationToken = default)
  {
    using var _ = await _semaphore.LockAsync(cancellationToken).ConfigureAwait(false);

    if (Done)
    {
      return _scanState;
    }

    var lastState = _scanState;

    _scanState = await _scans.GetScan(Id).ConfigureAwait(false);

    ChangingStatus(lastState.Status, _scanState.Status);

    return _scanState;
  }

  private void ChangingStatus(ScanStatus from, ScanStatus to)
  {
    if (from != ScanStatus.Queued && to == ScanStatus.Queued)
    {
      _logger.LogWarning(
        "The maximum amount of concurrent scans has been reached for the organization, " +
        "the execution will resume once a free engine will be available. " +
        "If you want to increase the execution concurrency, " +
        "please upgrade your subscription or contact your system administrator"
      );
    }

    if (from == ScanStatus.Queued && to != ScanStatus.Queued)
    {
      _logger.LogInformation("Connected to engine, resuming execution");
    }
  }
}
