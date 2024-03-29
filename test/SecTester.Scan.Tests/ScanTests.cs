using SecTester.Scan.Tests.Mocks;

namespace SecTester.Scan.Tests;

public class ScanTests : IAsyncDisposable
{
  private const string ScanId = "roMq1UVuhPKkndLERNKnA8";
  private const string IssueId = "pDzxcEXQC8df1fcz1QwPf9";

  private readonly Issue _issue = new(IssueId,
    "Cross-site request forgery is a type of malicious website exploit.",
    "Database connection crashed",
    "The best way to protect against those kind of issues is making sure the Database resources are sufficient",
    new Request("https://brokencrystals.com/")
    {
      Method = HttpMethod.Get
    },
    new Request("https://brokencrystals.com/")
    {
      Method = HttpMethod.Get
    },
    1,
    Severity.Medium,
    Protocol.Http,
    DateTime.UtcNow)
  {
    Cvss = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:N/I:N/A:L"
  };

  private readonly MockLogger<Scan> _logger = Substitute.For<MockLogger<Scan>>();
  private readonly Func<IScan, Task<bool>> _predicate = Substitute.For<Func<IScan, Task<bool>>>();
  private readonly IScans _scans = Substitute.For<IScans>();
  private readonly Scan _sut;

  public ScanTests()
  {
    _sut = new Scan(ScanId, _scans, _logger,
      new ScanOptions
      {
        Timeout = TimeSpan.FromSeconds(1),
        PollingInterval = TimeSpan.Zero,
        DeleteOnDispose = true
      });
  }

  public static IEnumerable<object[]> FailedStatuses =>
    new List<object[]>
    {
      new object[]
      {
        ScanStatus.Disrupted
      },
      new object[]
      {
        ScanStatus.Failed
      },
      new object[]
      {
        ScanStatus.Stopped
      }
    };

  public static IEnumerable<object[]> DoneStatuses =>
    new List<object[]>
    {
      new object[]
      {
        ScanStatus.Done
      }
    }.Concat(FailedStatuses);

  public static IEnumerable<object[]> ActiveStatuses =>
    new List<object[]>
    {
      new object[]
      {
        ScanStatus.Pending
      },
      new object[]
      {
        ScanStatus.Running
      },
      new object[]
      {
        ScanStatus.Queued
      }
    };

  public async ValueTask DisposeAsync()
  {
    await _sut.DisposeAsync();

    _predicate.ClearSubstitute();
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
    var act = () => new Scan(ScanId, null!, _logger, new ScanOptions());

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*scans*");
  }

  [Fact]
  public void Constructor_GivenNullLogger_ThrowError()
  {
    // act
    var act = () => new Scan(ScanId, _scans, null!, new ScanOptions());

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*logger*");
  }

  [Fact]
  public void Constructor_GivenNullScanOptions_ThrowError()
  {
    // act
    var act = () => new Scan(ScanId, _scans, _logger, null!);

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

  [Fact]
  public async Task Status_PollingCanceled_ReThrowsError()
  {
    // arrange
    using var cts = new CancellationTokenSource();

    cts.Cancel();

    // act
    var act = async () => await _sut.Status(cts.Token).FirstAsync(CancellationToken.None);

    // assert
    await act.Should().ThrowAsync<OperationCanceledException>();
  }

  [Theory]
  [MemberData(nameof(DoneStatuses))]
  public async Task Status_FinalStateReached_StopsQueryingState(ScanStatus scanStatus)
  {
    // arrange
    var scanState = new ScanState(scanStatus);

    _scans.GetScan(ScanId)
      .Returns(new ScanState(ScanStatus.Running), scanState);

    // act
    var result = await _sut.Status().LastAsync();

    // assert
    result.Should().BeEquivalentTo(scanState);
    await _scans.Received(2).GetScan(ScanId);
  }

  [Theory]
  [MemberData(nameof(DoneStatuses))]
  public async Task Status_FinalStateReached_YieldsFinalState(ScanStatus scanStatus)
  {
    // arrange
    var scanState = new ScanState(scanStatus);

    _scans.GetScan(ScanId)
      .Returns(new ScanState(ScanStatus.Running), scanState);

    // act
    var result = await _sut.Status().Take(5).LastAsync();

    // assert
    result.Should().BeEquivalentTo(scanState);
    await _scans.Received(2).GetScan(ScanId);
  }

  [Theory]
  [MemberData(nameof(ActiveStatuses))]
  public async Task Status_ScanIsActive_ContinuesQueryingState(ScanStatus scanStatus)
  {
    // arrange
    var scanState = new ScanState(scanStatus);

    _scans.GetScan(ScanId).Returns(scanState);

    // act
    var result = await _sut.Status().Take(5).LastOrDefaultAsync();

    // assert
    result.Should().BeEquivalentTo(scanState);
    await _scans.Received(5).GetScan(ScanId);
  }

  [Fact]
  public async Task Status_EntersIntoQueued_LogsConcurrencyWarning()
  {
    // arrange
    _scans.GetScan(ScanId).Returns(new ScanState(ScanStatus.Queued),
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
    _scans.GetScan(ScanId).Returns(
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
    var issues = new List<Issue>
    {
      _issue
    };
    _scans.ListIssues(ScanId)
      .Returns(issues);

    // act
    var result = await _sut.Issues();

    // assert
    result.Should().BeEquivalentTo(issues);
  }

  [Theory]
  [MemberData(nameof(ActiveStatuses))]
  public async Task Stop_ScanIsActive_StopsScan(ScanStatus scanStatus)
  {
    // arrange
    _scans.GetScan(ScanId).Returns(new ScanState(scanStatus));

    // act
    await _sut.Stop();

    // assert
    await _scans.Received(1).StopScan(ScanId);
  }

  [Theory]
  [MemberData(nameof(DoneStatuses))]
  public async Task Stop_ScanIsDone_DoesNothing(ScanStatus scanStatus)
  {
    // arrange
    _scans.GetScan(ScanId).Returns(new ScanState(scanStatus));

    // act
    await _sut.Stop();

    // assert
    await _scans.DidNotReceive().StopScan(ScanId);
  }

  [Fact]
  public async Task Stop_GetScanThrows_DoesNothing()
  {
    // arrange
    _scans.GetScan(ScanId).Throws(new ArgumentException());

    // act
    var act = () => _sut.Stop();

    // assert
    await act.Should().NotThrowAsync();
  }

  [Fact]
  public async Task Stop_StopScanThrows_DoesNothing()
  {
    // arrange
    _scans.GetScan(ScanId).Returns(new ScanState(ScanStatus.Done));
    _scans.StopScan(ScanId).Throws(new ArgumentException());

    // act
    var act = () => _sut.Stop();

    // assert
    await act.Should().NotThrowAsync();
  }

  [Theory]
  [MemberData(nameof(ActiveStatuses))]
  [MemberData(nameof(DoneStatuses))]
  public async Task DisposeAsync_DeleteOnDisposeIsTrue_DeletesFromApp(ScanStatus scanStatus)
  {
    // arrange
    _scans.GetScan(ScanId).Returns(new ScanState(scanStatus));

    // act
    await _sut.DisposeAsync();

    // assert
    await _scans.Received(1).DeleteScan(ScanId);
  }

  [Theory]
  [MemberData(nameof(ActiveStatuses))]
  [MemberData(nameof(DoneStatuses))]
  public async Task DisposeAsync_DefaultDeleteOnDispose_DoesNothing(ScanStatus scanStatus)
  {
    // arrange
    var sut = new Scan(ScanId, _scans, _logger,
      new ScanOptions
      {
        Timeout = TimeSpan.Zero,
        PollingInterval = TimeSpan.Zero
      });

    await using var _ = sut;

    _scans.GetScan(ScanId).Returns(new ScanState(scanStatus));

    // act
    await sut.DisposeAsync();

    // assert
    await _scans.DidNotReceive().DeleteScan(ScanId);
  }

  [Fact]
  public async Task DisposeAsync_DeleteScanThrows_DoesNothing()
  {
    // arrange
    _scans.GetScan(ScanId).Returns(new ScanState(ScanStatus.Done));
    _scans.DeleteScan(ScanId).Throws(new ArgumentException());

    // act
    var act = () => _sut.DisposeAsync().AsTask();

    // assert
    await act.Should().NotThrowAsync();
    await _scans.Received(1).DeleteScan(ScanId);
  }

  [Fact]
  public async Task Expect_ScanCompletedWithDoneStatus_Returns()
  {
    // arrange
    _scans.GetScan(ScanId).Returns(
      new ScanState(ScanStatus.Running), new ScanState(ScanStatus.Done));
    _predicate(Arg.Is<IScan>(x => x.Id == ScanId)).Returns(false);

    // act
    var act = () => _sut.Expect(_predicate);

    // assert
    await act.Should().NotThrowAsync();
    await _predicate.Received(2)(Arg.Any<IScan>());
  }

  [Theory]
  [MemberData(nameof(FailedStatuses))]
  public async Task Expect_ScanCompletedWithStatusDifferentFromDone_ThrowsException(ScanStatus scanStatus)
  {
    // arrange
    _scans.GetScan(ScanId).Returns(
      new ScanState(ScanStatus.Running), new ScanState(scanStatus));
    _predicate(Arg.Is<IScan>(x => x.Id == ScanId)).Returns(false);

    // act
    var act = () => _sut.Expect(_predicate);

    // assert
    await act.Should().ThrowAsync<ScanAborted>();
    await _predicate.Received(2)(Arg.Any<IScan>());
  }

  [Fact]
  public async Task Expect_GivenCustomPredicate_CancelledByTimeout_ThrowsException()
  {
    // arrange
    var sut = new Scan(ScanId, _scans, _logger,
      new ScanOptions
      {
        PollingInterval = TimeSpan.FromMilliseconds(200),
        Timeout = TimeSpan.Zero
      });
    await using var _ = sut;

    _scans.GetScan(ScanId).Returns(new ScanState(ScanStatus.Running));
    _predicate(Arg.Is<IScan>(x => x.Id == ScanId)).Returns(false);

    // act
    var act = () => sut.Expect(_predicate);

    // assert
    await act.Should().ThrowAsync<ScanTimedOut>();
    await _predicate.Received(0)(Arg.Any<IScan>());
  }

  [Fact]
  public async Task Expect_GivenCustomPredicate_CancellationTokenIsCancelled_Returns()
  {
    // arrange
    using var cancelledTokenSource = new CancellationTokenSource();
    cancelledTokenSource.Cancel();

    var sut = new Scan(ScanId, _scans, _logger,
      new ScanOptions
      {
        PollingInterval = TimeSpan.Zero,
        Timeout = TimeSpan.FromSeconds(1)
      });

    await using var _ = sut;

    _scans.GetScan(ScanId).Returns(new ScanState(ScanStatus.Running));
    _predicate(Arg.Is<IScan>(x => x.Id == ScanId)).Returns(false);

    // act
    var act = () => sut.Expect(_predicate, cancelledTokenSource.Token);

    // assert
    await act.Should().NotThrowAsync();
    await _predicate.Received(0)(Arg.Any<IScan>());
  }

  [Theory]
  [MemberData(nameof(ActiveStatuses))]
  public async Task Expect_CustomConditionSatisfied_Returns(ScanStatus scanStatus)
  {
    // arrange
    _scans.GetScan(ScanId).Returns(new ScanState(scanStatus));
    _predicate(Arg.Is<IScan>(x => x.Id == ScanId)).Returns(true);

    // act
    var act = () => _sut.Expect(_predicate);

    // assert
    await act.Should().NotThrowAsync();
    await _predicate.Received(1)(Arg.Any<IScan>());
  }

  [Theory]
  [MemberData(nameof(ActiveStatuses))]
  public async Task Expect_ComplexPredicateGiven_Returns(ScanStatus scanStatus)
  {
    // arrange
    var satisfyingScanState = new ScanState(scanStatus)
    {
      IssuesBySeverity = new[]
      {
        new IssueGroup(1, Severity.High)
      }
    };

    _predicate(Arg.Is<IScan>(x => x.Id == ScanId)).Returns(async _ =>
    {
      var status = await _sut.Status().FirstAsync();
      return status.IssuesBySeverity?.Any(x => x.Type == Severity.High) ?? false;
    });

    _scans.GetScan(ScanId).Returns(new ScanState(scanStatus), satisfyingScanState);

    // act
    var act = () => _sut.Expect(_predicate);

    // assert
    await act.Should().NotThrowAsync();
    await _predicate.Received(1)(Arg.Any<IScan>());
  }

  [Fact]
  public async Task Expect_PredicateIsNull_ThrowError()
  {
    // act
    var act = () => _sut.Expect(null!);

    // assert
    await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("*predicate*");
  }

  [Fact]
  public async Task Expect_PredicateThrows_DoesNothing()
  {
    _scans.GetScan(ScanId).Returns(new ScanState(ScanStatus.Done));
    _predicate(Arg.Is<IScan>(x => x.Id == ScanId)).Throws<NullReferenceException>();

    // act
    var act = () => _sut.Expect(_predicate);

    // assert
    await act.Should().NotThrowAsync();
    await _predicate.Received(1)(Arg.Any<IScan>());
  }

  [Fact]
  public async Task Expect_CancelledByTimeout_Returns()
  {
    // arrange
    var sut = new Scan(ScanId, _scans, _logger,
      new ScanOptions
      {
        PollingInterval = TimeSpan.FromMilliseconds(200),
        Timeout = TimeSpan.Zero
      });
    await using var _ = sut;

    _scans.GetScan(ScanId).Returns(new ScanState(ScanStatus.Running));

    // act
    var act = () => sut.Expect(Severity.High);

    // assert
    await act.Should().ThrowAsync<ScanTimedOut>();
  }

  [Fact]
  public async Task Expect_CancellationTokenIsCancelled_Returns()
  {
    // arrange
    using var cancelledTokenSource = new CancellationTokenSource();
    cancelledTokenSource.Cancel();

    _scans.GetScan(ScanId).Returns(new ScanState(ScanStatus.Running));

    // act
    var act = () => _sut.Expect(Severity.High, cancelledTokenSource.Token);

    // assert
    await act.Should().NotThrowAsync<Exception>();
  }

  [Theory]
  [MemberData(nameof(ActiveStatuses))]
  public async Task Expect_ConditionSatisfied_Returns(ScanStatus scanStatus)
  {
    // arrange
    _scans.GetScan(ScanId).Returns(new ScanState(scanStatus), new ScanState(scanStatus)
    {
      IssuesBySeverity = new[] { new IssueGroup(1, Severity.Critical) }
    });

    // act
    var act = () => _sut.Expect(Severity.Low);

    // assert
    await act.Should().NotThrowAsync<Exception>();
  }
}
