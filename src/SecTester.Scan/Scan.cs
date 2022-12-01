using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SecTester.Scan.Internal;
using SecTester.Scan.Models;

namespace SecTester.Scan;

public class Scan : IAsyncDisposable
{
  private static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromSeconds(5);

  private static readonly IReadOnlyCollection<ScanStatus> ActiveStatuses =
    new[] { ScanStatus.Pending, ScanStatus.Running, ScanStatus.Queued };

  private static readonly IReadOnlyCollection<ScanStatus> DoneStatuses =
    new[] { ScanStatus.Disrupted, ScanStatus.Done, ScanStatus.Failed, ScanStatus.Stopped };

  private readonly ScanOptions _options;
  private readonly ILogger _logger;
  private readonly Scans _scans;
  private ScanState _scanState = new(ScanStatus.Pending);
  private readonly SemaphoreSlim _scanStateSemaphore = new(1, 1);

  public string Id { get; }

  private bool InternalDone => DoneStatuses.Contains(_scanState.Status);
  private bool InternalActive => ActiveStatuses.Contains(_scanState.Status);

  public bool Active
  {
    get
    {
      using var _ = SemaphoreLock.Lock(_scanStateSemaphore);
      return InternalActive;
    }
  }

  public bool Done
  {
    get
    {
      using var _ = SemaphoreLock.Lock(_scanStateSemaphore);
      return InternalDone;
    }
  }

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
      await RefreshState().ConfigureAwait(false);

      if (!Active)
      {
        await _scans.DeleteScan(Id).ConfigureAwait(false);
      }
    }
    catch
    {
      // ignore
    }

    _scanStateSemaphore.Dispose();

    GC.SuppressFinalize(this);
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
      await Task.Delay(_options.PollingInterval ?? DefaultPollingInterval, cancellationToken).ConfigureAwait(false);

      yield return await RefreshState(cancellationToken).ConfigureAwait(false);
    }
  }

  private async Task<ScanState> RefreshState(CancellationToken cancellationToken = default)
  {
    ScanState newState;

    if (_scanStateSemaphore.CurrentCount == 0)
    {
      using var _ = await SemaphoreLock.LockAsync(_scanStateSemaphore, cancellationToken).ConfigureAwait(false);
      newState = _scanState;
    }
    else
    {
      ScanState lastState;

      using (await SemaphoreLock.LockAsync(_scanStateSemaphore, cancellationToken).ConfigureAwait(false))
      {
        if (InternalDone)
        {
          return _scanState;
        }

        lastState = _scanState;

        newState = await _scans.GetScan(Id).ConfigureAwait(false);

        _scanState = newState;
      }

      ChangingStatus(lastState.Status, newState.Status);
    }

    return newState;
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
