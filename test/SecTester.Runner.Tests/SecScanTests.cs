namespace SecTester.Runner.Tests;

public class SecScanTests : IDisposable
{
  private const string Url = "https://example.com";
  private readonly Formatter _formatter = Substitute.For<Formatter>();

  private readonly Issue _issue = new("pDzxcEXQC8df1fcz1QwPf9", "Cross-site request forgery is a type of malicious website exploit.",
    "Database connection crashed",
    "The best way to protect against those kind of issues is making sure the Database resources are sufficient",
    new Request("https://brokencrystals.com/"), new Request("https://brokencrystals.com/"), 1, Severity.Medium, Protocol.Http,
    DateTime.Today);
  private readonly IScan _scan = Substitute.For<IScan>();
  private readonly ScanFactory _scanFactory = Substitute.For<ScanFactory>();
  private readonly SecScan _sut;

  private readonly Target _target = new(Url);
  private readonly IEnumerable<TestType> _tests = new List<TestType> { TestType.HeaderSecurity };

  private readonly TimeSpan _timeout = TimeSpan.FromHours(1);

  public SecScanTests()
  {
    var builder = new ScanSettingsBuilder().WithTests(_tests);
    _sut = new SecScan(builder, _scanFactory, _formatter);
  }

  public void Dispose()
  {
    _scan.ClearSubstitute();
    _formatter.ClearSubstitute();
    _scanFactory.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task Run_StartsScan()
  {
    // act
    await _sut.Run(_target);

    // assert
    await _scanFactory.Received(1)
      .CreateScan(
        Arg.Is<ScanSettings>(x => x.Tests.SequenceEqual(_tests) && x.Target == _target),
        Arg.Is<ScanOptions?>(options => options!.Timeout == TimeSpan.FromMinutes(10)));
  }

  [Fact]
  public async Task Run_StartsScanWithDefaultThreshold()
  {
    // arrange
    _scanFactory.CreateScan(Arg.Any<ScanSettings>(), Arg.Any<ScanOptions?>()).Returns(_scan);

    // act
    await _sut.Run(_target);

    // assert
    await _scan.Received(1).Expect(Severity.Low, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Run_StartsScanWithCustomThreshold()
  {
    // arrange
    _scanFactory.CreateScan(Arg.Any<ScanSettings>(), Arg.Any<ScanOptions?>()).Returns(_scan);
    _sut.Threshold(Severity.High);

    // act
    await _sut.Run(_target);

    // assert
    await _scan.Received(1).Expect(Severity.High, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Run_StartsScanWithTimeout()
  {
    // arrange
    _scanFactory.CreateScan(Arg.Any<ScanSettings>(), Arg.Any<ScanOptions?>()).Returns(_scan);
    _sut.Timeout(_timeout);

    // act
    await _sut.Run(_target);

    // assert
    await _scanFactory.Received(1).CreateScan(Arg.Any<ScanSettings>(), Arg.Is<ScanOptions?>(x => x!.Timeout == _timeout));
  }

  [Fact]
  public async Task Run_StopsScanAtEnd()
  {
    // arrange
    _scanFactory.CreateScan(Arg.Any<ScanSettings>(), Arg.Any<ScanOptions?>()).Returns(_scan);

    // act
    await _sut.Run(_target);

    // assert
    await _scan.Received(1).Stop();
  }

  [Fact]
  public async Task Run_ThrowsIssueFoundExceptionWhenExpectedIssueIsFound()
  {
    // arrange
    _scanFactory.CreateScan(Arg.Any<ScanSettings>(), Arg.Any<ScanOptions?>()).Returns(_scan);
    _scan.Issues().Returns(new List<Issue> { _issue });

    // act
    var act = () => _sut.Run(_target);

    // assert
    await act.Should().ThrowAsync<IssueFound>();
  }

  [Fact]
  public async Task Run_DoesNothingWhenThereIsNoIssue()
  {
    // arrange
    _scanFactory.CreateScan(Arg.Any<ScanSettings>(), Arg.Any<ScanOptions?>()).Returns(_scan);

    // act
    var act = () => _sut.Run(_target);

    // assert
    await act.Should().NotThrowAsync<IssueFound>();
  }

  [Fact]
  public async Task Threshold_SetsExpectedSeverityThreshold()
  {
    // arrange
    var builder = new ScanSettingsBuilder().WithTests(_tests);
    var sut = new SecScan(builder, _scanFactory, _formatter);
    _scanFactory.CreateScan(Arg.Any<ScanSettings>(), Arg.Any<ScanOptions?>()).Returns(_scan);

    // act
    await sut.Threshold(Severity.High).Run(_target);

    // assert
    await _scan.Received(1).Expect(Severity.High);
  }

  [Fact]
  public async Task Timeout_SetsExpectedScanTimeout()
  {
    // arrange
    var builder = new ScanSettingsBuilder().WithTests(_tests);
    var sut = new SecScan(builder, _scanFactory, _formatter);
    _scanFactory.CreateScan(Arg.Any<ScanSettings>(), Arg.Any<ScanOptions?>()).Returns(_scan);

    // act
    await sut.Timeout(_timeout).Run(_target);

    // assert
    await _scanFactory.Received(1).CreateScan(Arg.Any<ScanSettings>(), Arg.Is<ScanOptions>(options => options.Timeout == _timeout));
  }
}
