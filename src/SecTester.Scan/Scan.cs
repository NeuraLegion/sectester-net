using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
  private volatile ScanState _scanState = new(ScanStatus.Pending);

  public string Id { get; init; }

  public Scan(string id, Scans scans, ILogger logger, ScanOptions options)
  {
    Id = id ?? throw new ArgumentNullException(nameof(id));
    _scans = scans ?? throw new ArgumentNullException(nameof(scans));
    _options = options ?? throw new ArgumentNullException(nameof(options));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public bool Active => ActiveStatuses.Contains(_scanState.Status);

  public bool Done => DoneStatuses.Contains(_scanState.Status);

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
      // noop
    }

    GC.SuppressFinalize(this);
  }


  public Task<IEnumerable<Issue>> Issues()
  {
    return _scans.ListIssues(Id);
  }

  public Task Stop()
  {
    return _scans.StopScan(Id);
  }

  public async IAsyncEnumerable<ScanState> Status(
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    while (Active)
    {
      await Task.Delay(_options.PollingInterval ?? DefaultPollingInterval, cancellationToken).ConfigureAwait(false);

      yield return await RefreshState().ConfigureAwait(false);
    }
  }

  private async Task<ScanState> RefreshState()
  {
    if (this.Done)
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
