using Microsoft.Extensions.Logging;
using NSubstitute.ClearExtensions;
using SecTester.Scan.Tests.Fixtures;

namespace SecTester.Scan.Tests;

public class ScanTests : ScanFixture
{
  private readonly Scans _scans = Substitute.For<Scans>();
  private readonly LoggerMock _logger = Substitute.For<LoggerMock>();

  public override void Dispose()
  {
    _scans.ClearSubstitute();
    _logger.ClearSubstitute();

    base.Dispose();
  }

  [Fact]
  public void Constructor_GivenNullId_ThrowError()
  {
    // act
    Action act = () => new Scan(null!, _scans, _logger, new ScanOptions());

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*id*");
  }

  [Fact]
  public void Constructor_GivenNullScans_ThrowError()
  {
    // act
    Action act = () => new Scan(ScanId, null!, _logger, new ScanOptions());

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*scans*");
  }

  [Fact]
  public void Constructor_GivenNullLogger_ThrowError()
  {
    // act
    Action act = () => new Scan(ScanId, _scans, null!, new ScanOptions());

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*logger*");
  }

  [Fact]
  public void Constructor_GivenNullScanOptions_ThrowError()
  {
    // act
    Action act = () => new Scan(ScanId, _scans, _logger, null!);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*options*");
  }

  [Fact]
  public async Task Active_DefaultValue_ReturnsTrue()
  {
    // act
    await using var scan = new Scan(ScanId, _scans, _logger, new ScanOptions());

    // assert
    scan.Active.Should().BeTrue();
  }

  [Fact]
  public async Task Done_DefaultValue_ReturnsTrue()
  {
    // act
    await using var scan = new Scan(ScanId, _scans, _logger, new ScanOptions());

    // assert
    scan.Done.Should().BeFalse();
  }

  [Fact]
  public async Task Status_ReturnsState()
  {
    // arrange
    var scanState = new ScanState(ScanStatus.Running);
    await using var scan = new Scan(ScanId, _scans, _logger, new ScanOptions(PollingInterval: TimeSpan.Zero));

    _scans.GetScan(ScanId).Returns(Task.FromResult(scanState));

    // act
    var result = await scan.Status().FirstAsync();

    // assert
    result.Should().Be(scanState);
  }

  [Fact]
  public async Task Status_GivenCancelledToken_ThrowError()
  {
    // arrange
    using var cts = new CancellationTokenSource();
    await using var scan = new Scan(ScanId, _scans, _logger, new ScanOptions(PollingInterval: TimeSpan.Zero));

    cts.Cancel();

    // act
    var act = async () => await scan.Status(cts.Token).FirstAsync();

    // assert
    await act.Should().ThrowAsync<OperationCanceledException>();
  }

  [Fact]
  public async Task Status_SwitchingDoneState_StopsYield()
  {
    // arrange
    var runningScanState = new ScanState(ScanStatus.Running);
    var stoppedScanState = new ScanState(ScanStatus.Stopped);

    await using var scan = new Scan(ScanId, _scans, _logger, new ScanOptions(PollingInterval: TimeSpan.Zero));

    _scans.GetScan(ScanId).Returns(Task.FromResult(runningScanState),
      Task.FromResult(stoppedScanState));

    // act
    var result = await scan.Status().LastOrDefaultAsync();

    // assert
    result.Should().BeEquivalentTo(stoppedScanState);
  }

  [Fact]
  public async Task Status_GivenQueuedStateConsequently_LogsConcurrencyWarning()
  {
    // arrange
    var scanState = new ScanState(ScanStatus.Queued);

    await using var scan = new Scan(ScanId, _scans, _logger, new ScanOptions(PollingInterval: TimeSpan.Zero));

    _scans.GetScan(ScanId).Returns(Task.FromResult(scanState),
      Task.FromResult(scanState));

    // act
    await scan.Status().Take(2).LastOrDefaultAsync();

    // assert
    _logger.Received(1).Log(LogLevel.Warning, Arg.Is<string>(s => s.Contains("increase the execution concurrency")));
  }

  [Fact]
  public async Task Status_GivenAnyStateAfterQueued_LogsResumingExecution()
  {
    // arrange
    var queuedState = new ScanState(ScanStatus.Queued);
    var pokeState = new ScanState(ScanStatus.Running);

    await using var scan = new Scan(ScanId, _scans, _logger, new ScanOptions(PollingInterval: TimeSpan.Zero));

    _scans.GetScan(ScanId).Returns(
      Task.FromResult(queuedState), Task.FromResult(pokeState));

    // act
    await scan.Status().Take(2).LastOrDefaultAsync();

    // assert
    _logger.Received(1).Log(LogLevel.Information, "Connected to engine, resuming execution");
  }

  [Fact]
  public async Task Issues_ReturnsIssues()
  {
    // arrange
    await using var scan = new Scan(ScanId, _scans, _logger, new ScanOptions(PollingInterval: TimeSpan.Zero));

    _scans.ListIssues(ScanId)
      .Returns(Task.FromResult(Issues));

    // act
    var result = await scan.Issues();

    // assert
    result.Should().BeEquivalentTo(Issues);
  }

  [Fact]
  public async Task Stop_StopsScan()
  {
    // arrange
    await using var scan = new Scan(ScanId, _scans, _logger, new ScanOptions(PollingInterval: TimeSpan.Zero));

    // act
    await scan.Stop();

    // assert
    await _scans.Received(1).StopScan(ScanId);
  }

  [Fact]
  public async Task DisposeAsync_ActiveIsFalse_DeletesScan()
  {
    // arrange
    var scanState = new ScanState(ScanStatus.Stopped);
    var scan = new Scan(ScanId, _scans, _logger, new ScanOptions(PollingInterval: TimeSpan.Zero));

    _scans.GetScan(ScanId).Returns(Task.FromResult(scanState));

    // act
    await scan.DisposeAsync();

    // assert
    await _scans.Received(1).DeleteScan(ScanId);
  }

  [Fact]
  public async Task DisposeAsync_ActiveIsTrue_KeepsScan()
  {
    // arrange
    var scanState = new ScanState(ScanStatus.Running);
    var scan = new Scan(ScanId, _scans, _logger, new ScanOptions(PollingInterval: TimeSpan.Zero));

    _scans.GetScan(ScanId).Returns(Task.FromResult(scanState));

    // act
    await scan.DisposeAsync();

    // assert
    await _scans.DidNotReceive().DeleteScan(ScanId);
  }
}
