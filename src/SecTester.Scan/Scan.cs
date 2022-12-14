using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SecTester.Core.Extensions;
using SecTester.Scan.Exceptions;
using SecTester.Scan.Models;

namespace SecTester.Scan;

public class Scan : IScan
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

  private readonly ILogger _logger;

  private readonly ScanOptions _options;
  private readonly IScans _scans;
  private readonly SemaphoreSlim _semaphore = new(1, 1);
  private ScanState _state = new(ScanStatus.Pending);

  public Scan(string id, IScans scans, ILogger<Scan> logger, ScanOptions options)
  {
    Id = id ?? throw new ArgumentNullException(nameof(id));
    _scans = scans ?? throw new ArgumentNullException(nameof(scans));
    _options = options ?? throw new ArgumentNullException(nameof(options));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public string Id { get; }

  public bool Active => ActiveStatuses.Contains(_state.Status);

  public bool Done => DoneStatuses.Contains(_state.Status);

  private bool Failed => Done && _state.Status != ScanStatus.Done;

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
    if (_options.DeleteOnDispose)
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

    yield return _state;
  }

  public async Task Expect(Severity expectation, CancellationToken cancellationToken = default)
  {
    Task<bool> Predicate(IScan _) => Task.FromResult(IsInExpectedSeverityRange(expectation));
    await Expect(Predicate, cancellationToken).ConfigureAwait(false);
  }

  public async Task Expect(Func<IScan, Task<bool>> predicate, CancellationToken cancellationToken = default)
  {
    if (predicate == null)
    {
      throw new ArgumentNullException(nameof(predicate));
    }

    await ExpectCore(predicate, cancellationToken).ConfigureAwait(false);
  }

  private async Task ExpectCore(Func<IScan, Task<bool>> predicate, CancellationToken cancellationToken)
  {
    using var cancellationTokenSource =
      CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

    cancellationTokenSource.CancelAfter(_options.Timeout);

    try
    {
      await PollStatusUntil(predicate, cancellationTokenSource.Token).ConfigureAwait(false);
    }
    catch (OperationCanceledException exception)
    {
      Assert(!cancellationToken.IsCancellationRequested, exception);
    }

    Assert();
  }

  private void Assert()
  {
    if (Failed)
    {
      throw new ScanAborted(_state.Status);
    }
  }

  private void Assert(bool timeoutPassed, Exception? exception = default)
  {
    if (timeoutPassed)
    {
      throw new ScanTimedOut(_options.Timeout, exception);
    }
  }

  private async Task PollStatusUntil(Func<IScan, Task<bool>> predicate, CancellationToken cancellationToken)
  {
    await Status(cancellationToken)
      .FirstOrDefaultAwaitAsync(
        async _ => await ApplyPredicate(predicate).ConfigureAwait(false) || Done, cancellationToken)
      .ConfigureAwait(false);
  }

  private bool IsInExpectedSeverityRange(Severity expectation)
  {
    return _state.IssuesBySeverity switch
    {
      null => false,
      _ => _state.IssuesBySeverity.Any(x => x.Type >= expectation && x.Number > 0)
    };

  }

  private async Task<bool> ApplyPredicate(Func<IScan, Task<bool>> predicate)
  {
    try
    {
      return await predicate(this).ConfigureAwait(false);
    }
    catch
    {
      return false;
    }
  }

  private async Task<ScanState> RefreshState(CancellationToken cancellationToken = default)
  {
    using var _ = await _semaphore.LockAsync(cancellationToken).ConfigureAwait(false);

    if (Done)
    {
      return _state;
    }

    var lastState = _state;

    _state = await _scans.GetScan(Id).ConfigureAwait(false);

    ChangingStatus(lastState.Status, _state.Status);

    return _state;
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
