namespace SecTester.Scan.Tests;

public class DefaultScansTests : IDisposable
{
  private const string NullResultMessage = "Something went wrong. Please try again later.";
  private const string BaseUrl = "https://example.com/api/v1";
  private const string ScanName = "Scan Name";
  private const string ProjectId = "e9a2eX46EkidKhn3uqdYvE";
  private const string RepeaterId = "g5MvgM74sweGcK1U6hvs76";
  private const string FileId = "6aJa25Yd8DdXEcZg3QFoi8";
  private const string ScanId = "roMq1UVuhPKkndLERNKnA8";
  private const string IssueId = "pDzxcEXQC8df1fcz1QwPf9";
  private const string HarId = "gwycPnxzQihoeGP141pvDe";
  private const string HarFileName = "filename.har";

  private readonly ScanConfig _scanConfig = new(ScanName)
  {
    Module = Module.Dast,
    Repeaters = new[] { RepeaterId },
    Smart = true,
    Tests = new[] { TestType.Csrf, TestType.Jwt },
    DiscoveryTypes = new[] { Discovery.Crawler },
    FileId = FileId,
    HostsFilter = new[] { "example.com" },
    PoolSize = 2,
    ProjectId = ProjectId,
    TargetTimeout = 10,
    AttackParamLocations = new[] { AttackParamLocation.Body, AttackParamLocation.Header },
    SkipStaticParams = true,
    SlowEpTimeout = 20
  };

  private readonly Issue _issue = new(IssueId,
    "Cross-site request forgery is a type of malicious website exploit.",
    "Database connection crashed",
    "The best way to protect against those kind of issues is making sure the Database resources are sufficient",
    new Request("https://brokencrystals.com/") { Method = HttpMethod.Get },
    new Request("https://brokencrystals.com/") { Method = HttpMethod.Get },
    $"{BaseUrl}/scans/{ScanId}/issues/{IssueId}",
    1,
    Severity.Medium,
    Protocol.Http,
    DateTime.UtcNow)
  { Cvss = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:N/I:N/A:L" };

  private readonly Har _har = new(
    new Log(
      new Tool("name", "v1.1.1")
    )
  );

  private readonly Configuration _configuration = new("app.neuralegion.com");
  private readonly CommandDispatcher _commandDispatcher = Substitute.For<CommandDispatcher>();
  private readonly CiDiscovery _ciDiscovery = Substitute.For<CiDiscovery>();

  private readonly Scans _sut;

  public DefaultScansTests()
  {
    _sut = new DefaultScans(_configuration, _commandDispatcher, _ciDiscovery);
  }

  public void Dispose()
  {
    _ciDiscovery.ClearSubstitute();
    _commandDispatcher.ClearSubstitute();

    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task CreateScan_CreatesNewScan()
  {
    // arrange
    _commandDispatcher.Execute(Arg.Any<CreateScan>())
      .Returns(new Identifiable<string>(ScanId));

    // act 
    var result = await _sut.CreateScan(_scanConfig);

    // assert
    result.Should().Be(ScanId);
    await _commandDispatcher.Received(1)
      .Execute(Arg.Any<CreateScan>());
  }

  [Fact]
  public async Task CreateScan_ResultIsNull_ThrowError()
  {
    // arrange
    _commandDispatcher.Execute(Arg.Any<CreateScan>())
      .Returns(null as Identifiable<string>);

    // act 
    var act = () => _sut.CreateScan(_scanConfig);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }

  [Fact]
  public async Task ListIssues_ReturnListOfIssues()
  {
    // arrange
    var issues = new List<Issue> { _issue };
    _commandDispatcher.Execute(Arg.Any<ListIssues>()).Returns(issues);

    // act
    var result = await _sut.ListIssues(ScanId);

    // assert
    result.Should().BeEquivalentTo(issues);
    await _commandDispatcher.Received(1)
      .Execute(Arg.Any<ListIssues>());
  }

  [Fact]
  public async Task ListIssues_ResultIsNull_ThrowError()
  {
    // arrange
    _commandDispatcher.Execute(Arg.Any<ListIssues>())
      .Returns(Task.FromResult<IEnumerable<Issue>?>(null));

    // act 
    var act = () => _sut.ListIssues(ScanId);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }

  [Fact]
  public async Task StopScan_StopsScan()
  {
    // act
    await _sut.StopScan(ScanId);

    // assert
    await _commandDispatcher.Received(1)
      .Execute(Arg.Any<StopScan>());
  }

  [Fact]
  public async Task DeleteScan_DeletesScan()
  {
    // act
    await _sut.DeleteScan(ScanId);

    // assert
    await _commandDispatcher.Received(1)
      .Execute(Arg.Any<DeleteScan>());
  }

  [Fact]
  public async Task GetScan_ReturnsScanState()
  {
    // arrange
    var scanState = new ScanState(ScanStatus.Done);

    _commandDispatcher.Execute(Arg.Any<GetScan>())
      .Returns(scanState);

    // act
    var result = await _sut.GetScan(ScanId);

    // assert
    result.Should().Be(scanState);
    await _commandDispatcher.Received(1)
      .Execute(Arg.Any<GetScan>());
  }

  [Fact]
  public async Task GetScan_ResultIsNull_ThrowError()
  {
    // arrange
    _commandDispatcher.Execute(Arg.Any<GetScan>())
      .Returns(Task.FromResult<ScanState?>(null));

    // act 
    var act = () => _sut.GetScan(ScanId);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }

  [Fact]
  public async Task UploadHar_CreatesNewHar()
  {
    // arrange
    var options = new UploadHarOptions(_har, HarFileName);

    _commandDispatcher.Execute(Arg.Any<UploadHar>())
      .Returns(new Identifiable<string>(HarId));

    // act
    var result = await _sut.UploadHar(options);

    // assert
    result.Should().Be(HarId);
    await _commandDispatcher.Received(1)
      .Execute(Arg.Any<UploadHar>());
  }

  [Fact]
  public async Task UploadHar_ResultIsNull_ThrowError()
  {
    // arrange
    var options = new UploadHarOptions(_har, HarFileName);

    _commandDispatcher.Execute(Arg.Any<UploadHar>())
      .Returns(null as Identifiable<string>);

    // act 
    var act = () => _sut.UploadHar(options);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }
}
