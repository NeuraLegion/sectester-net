namespace SecTester.Scan.Tests;

public class ScanTests : IAsyncDisposable
{
  public static readonly IEnumerable<object[]> ActiveStatuses = ScanFixture.ActiveStatuses;
  public static readonly IEnumerable<object[]> DoneStatuses = ScanFixture.DoneStatuses;

  private readonly Scans _scans = Substitute.For<Scans>();
  private readonly LoggerMock _logger = Substitute.For<LoggerMock>();
  private readonly Scan _sut;

  public ScanTests()
  {
    _sut = new Scan(ScanFixture.ScanId, _scans, _logger, new ScanOptions(
      TimeSpan.Zero, TimeSpan.Zero));
  }

  public async ValueTask DisposeAsync()
  {
    await _sut.DisposeAsync();

    _scans.ClearSubstitute();
    _logger.ClearSubstitute();

    GC.SuppressFinalize(this);
  }

  [Fact]
  public void Constructor_GivenNullId_ThrowError()
  {
    // act
    var act = () => new Scan(null!, _scans, _logger, new ScanOptions());

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*id*");
  }

  [Fact]
  public void Constructor_GivenNullScans_ThrowError()
  {
    // act
    var act = () => new Scan(ScanFixture.ScanId, null!, _logger, new ScanOptions());

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*scans*");
  }

  [Fact]
  public void Constructor_GivenNullLogger_ThrowError()
  {
    // act
    var act = () => new Scan(ScanFixture.ScanId, _scans, null!, new ScanOptions());

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*logger*");
  }

  [Fact]
  public void Constructor_GivenNullScanOptions_ThrowError()
  {
    // act
    var act = () => new Scan(ScanFixture.ScanId, _scans, _logger, null!);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*options*");
  }

  [Fact]
  public void Active_DefaultValue_ReturnsTrue()
  {
    // assert
    _sut.Active.Should().BeTrue();
  }

  [Fact]
  public void Done_DefaultValue_ReturnsTrue()
  {
    // assert
    _sut.Done.Should().BeFalse();
  }

  [Theory]
  [MemberData(nameof(ActiveStatuses))]
  public async Task Stop_ScanIsActiveWithConcurrency_ReducesGetScanCalls(ScanStatus scanStatus)
  {
    // arrange
    _scans.GetScan(ScanFixture.ScanId).Returns(
      Task.Delay(80).ContinueWith(_ => new ScanState(scanStatus))
    );

    // act
    await Task.WhenAll(_sut.Stop(), _sut.Stop(), _sut.Stop(), _sut.Stop());

    // assert
    await _scans.Received(1).GetScan(ScanFixture.ScanId);
    await _scans.Received(4).StopScan(ScanFixture.ScanId);
  }

  [Theory]
  [MemberData(nameof(DoneStatuses))]
  public async Task Stop_ScanIsDoneWithConcurrency_DoesNothing(ScanStatus scanStatus)
  {
    // arrange
    _scans.GetScan(ScanFixture.ScanId).Returns(
      Task.FromResult(new ScanState(ScanStatus.Disrupted)),
      Task.Delay(80).ContinueWith(_ => new ScanState(scanStatus))
    );

    await _sut.Stop();

    // act
    await Task.WhenAll(_sut.Stop(), _sut.Stop(), _sut.Stop(), _sut.Stop());

    // assert
    await _scans.Received(1).GetScan(ScanFixture.ScanId);
    await _scans.DidNotReceive().StopScan(ScanFixture.ScanId);
  }

  [Fact]
  public async Task Status_PollingCanceled_ReThrowsError()
  {
    // arrange
    using var cts = new CancellationTokenSource();

    cts.Cancel();

    // act
    var act = async () => await _sut.Status(cts.Token).FirstAsync();

    // assert
    await act.Should().ThrowAsync<OperationCanceledException>();
  }

  [Theory]
  [MemberData(nameof(DoneStatuses))]
  public async Task Status_FinalStateReached_StopsQueryingState(ScanStatus scanStatus)
  {
    // arrange
    var scanState = new ScanState(scanStatus);

    _scans.GetScan(ScanFixture.ScanId)
      .Returns(new ScanState(ScanStatus.Running), scanState, new ScanState(ScanStatus.Running));

    // ADHOC: LastOrDefaultAsync() may hang-up when 'FinalState' condition is not satisfied   
    using var cts = new CancellationTokenSource();
    cts.CancelAfter(TimeSpan.FromSeconds(2));

    // act
    var result = await _sut.Status(cts.Token).LastOrDefaultAsync(cts.Token);

    // assert
    result.Should().BeEquivalentTo(scanState);
    await _scans.Received(2).GetScan(ScanFixture.ScanId);
  }

  [Theory]
  [MemberData(nameof(ActiveStatuses))]
  public async Task Status_ScanIsActive_ContinuesQueryingState(ScanStatus scanStatus)
  {
    // arrange
    var scanState = new ScanState(scanStatus);

    _scans.GetScan(ScanFixture.ScanId).Returns(scanState);

    // act
    var result = await _sut.Status().Take(5).LastOrDefaultAsync();

    // assert
    result.Should().BeEquivalentTo(scanState);
    await _scans.Received(5).GetScan(ScanFixture.ScanId);
  }

  [Fact]
  public async Task Status_EntersIntoQueued_LogsConcurrencyWarning()
  {
    // arrange
    _scans.GetScan(ScanFixture.ScanId).Returns(new ScanState(ScanStatus.Queued),
      new ScanState(ScanStatus.Queued));

    // act
    await _sut.Status().Take(2).LastOrDefaultAsync();

    // assert
    _logger.Received(1).Log(LogLevel.Warning, Arg.Is<string>(s => s.Contains("increase the execution concurrency")));
  }

  [Fact]
  public async Task Status_Dequeued_LogsResumingExecution()
  {
    // arrange
    _scans.GetScan(ScanFixture.ScanId).Returns(
      new ScanState(ScanStatus.Queued), new ScanState(ScanStatus.Running));

    // act
    await _sut.Status().Take(2).LastOrDefaultAsync();

    // assert
    _logger.Received(1).Log(LogLevel.Information, "Connected to engine, resuming execution");
  }

  [Fact]
  public async Task Issues_ReturnsIssues()
  {
    // arrange
    _scans.ListIssues(ScanFixture.ScanId)
      .Returns(ScanFixture.Issues);

    // act
    var result = await _sut.Issues();

    // assert
    result.Should().BeEquivalentTo(ScanFixture.Issues);
  }

  [Theory]
  [MemberData(nameof(ActiveStatuses))]
  public async Task Stop_ScanIsActive_StopsScan(ScanStatus scanStatus)
  {
    // arrange
    _scans.GetScan(ScanFixture.ScanId).Returns(new ScanState(scanStatus));

    // act
    await _sut.Stop();

    // assert
    await _scans.Received(1).StopScan(ScanFixture.ScanId);
  }

  [Theory]
  [MemberData(nameof(DoneStatuses))]
  public async Task Stop_ScanIsDone_DoesNothing(ScanStatus scanStatus)
  {
    // arrange
    _scans.GetScan(ScanFixture.ScanId).Returns(new ScanState(scanStatus));

    // act
    await _sut.Stop();

    // assert
    await _scans.DidNotReceive().StopScan(ScanFixture.ScanId);
  }

  [Fact]
  public async Task Stop_GetScanThrows_Returns()
  {
    // arrange
    _scans.GetScan(ScanFixture.ScanId).Throws(new ArgumentException());

    // act
    var act = () => _sut.Stop();

    // assert
    await act.Should().NotThrowAsync();
  }

  [Fact]
  public async Task Stop_StopScanThrows_Returns()
  {
    // arrange
    _scans.GetScan(ScanFixture.ScanId).Returns(new ScanState(ScanStatus.Done));
    _scans.StopScan(ScanFixture.ScanId).Throws(new ArgumentException());

    // act
    var act = () => _sut.Stop();

    // assert
    await act.Should().NotThrowAsync();
  }

  [Theory]
  [MemberData(nameof(DoneStatuses))]
  public async Task DisposeAsync_ScanIsDone_DeletesFromApp(ScanStatus scanStatus)
  {
    // arrange
    _scans.GetScan(ScanFixture.ScanId).Returns(new ScanState(scanStatus));

    // act
    await _sut.DisposeAsync();

    // assert
    await _scans.Received(1).DeleteScan(ScanFixture.ScanId);
  }

  [Theory]
  [MemberData(nameof(ActiveStatuses))]
  public async Task DisposeAsync_ScanIsActive_DoesNothing(ScanStatus scanStatus)
  {
    // arrange
    _scans.GetScan(ScanFixture.ScanId).Returns(new ScanState(scanStatus));

    // act
    await _sut.DisposeAsync();

    // assert
    await _scans.DidNotReceive().DeleteScan(ScanFixture.ScanId);
  }

  [Fact]
  public async Task DisposeAsync_GetScanThrows_Returns()
  {
    // arrange
    _scans.GetScan(ScanFixture.ScanId).Throws(new ArgumentException());

    // act
    var act = () => _sut.DisposeAsync().AsTask();

    // assert
    await act.Should().NotThrowAsync();
  }

  [Fact]
  public async Task DisposeAsync_DeleteScanThrows_Returns()
  {
    // arrange
    _scans.GetScan(ScanFixture.ScanId).Returns(new ScanState(ScanStatus.Done));
    _scans.DeleteScan(ScanFixture.ScanId).Throws(new ArgumentException());

    // act
    var act = () => _sut.DisposeAsync().AsTask();

    // assert
    await act.Should().NotThrowAsync();
  }
}
