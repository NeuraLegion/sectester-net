using NSubstitute.ClearExtensions;
using SecTester.Bus.Dispatchers;
using SecTester.Bus.Extensions;
using SecTester.Core;
using SecTester.Core.Bus;
using SecTester.Core.Exceptions;
using SecTester.Scan.CI;
using SecTester.Scan.Commands;
using SecTester.Scan.Tests.Extensions;

namespace SecTester.Scan.Tests;

public class DefaultScansTests : IDisposable
{
  private const string ScanId = "roMq1UVuhPKkndLERNKnA8";
  private const string IssueId = "pDzxcEXQC8df1fcz1QwPf9";
  private const string HarId = "gwycPnxzQihoeGP141pvDe";
  private const string NullResultMessage = "Something went wrong. Please try again later.";

  private readonly Configuration _configuration = new("app.neuralegion.com");
  private readonly MessageSerializer _messageSerializer = new DefaultMessageSerializer();
  private readonly CommandDispatcher _commandDispatcher = Substitute.For<CommandDispatcher>();
  private readonly CiDiscovery _ciDiscovery = Substitute.For<CiDiscovery>();

  private readonly ScanConfig _scanConfig = new("scan1")
  {
    Module = Module.Dast,
    Repeaters = new[] { "repeater1" },
    Smart = true,
    Tests = new[] { TestType.Csrf, TestType.Jwt },
    DiscoveryTypes = new[] { Discovery.Crawler },
    FileId = "fileId",
    HostsFilter = new[] { "example.com" },
    PoolSize = 2,
    ProjectId = "projectId",
    TargetTimeout = 10,
    AttackParamLocations = new[] { AttackParamLocation.Body, AttackParamLocation.Header },
    SkipStaticParams = true,
    SlowEpTimeout = 20
  };

  private readonly Scans _sut;

  public DefaultScansTests()
  {
    _ciDiscovery.Server.Returns(new CiServer("Some CI"));

    _sut = new DefaultScans(_configuration, _commandDispatcher, _messageSerializer, _ciDiscovery);
  }

  public void Dispose()
  {
    _ciDiscovery.ClearSubstitute();
    _commandDispatcher.ClearSubstitute();
  }

  [Fact]
  public async Task CreateScan_CreatesNewScan()
  {
    // arrange
    _commandDispatcher.Execute(Arg.Any<CreateScan>())
      .Returns(Task.FromResult<Identifiable<string>?>(new Identifiable<string>("id")));

    // act 
    var result = await _sut.CreateScan(_scanConfig);

    // assert
    result.Should().Be("id");
  }

  [Fact]
  public async Task CreateScan_PassesCreationInfoInThePayload()
  {
    // arrange
    var expectedPayload = new
    {
      _scanConfig.Name,
      _scanConfig.Module,
      _scanConfig.Tests,
      _scanConfig.DiscoveryTypes,
      _scanConfig.PoolSize,
      _scanConfig.AttackParamLocations,
      _scanConfig.FileId,
      _scanConfig.HostsFilter,
      _scanConfig.Repeaters,
      _scanConfig.Smart,
      _scanConfig.SkipStaticParams,
      _scanConfig.ProjectId,
      _scanConfig.SlowEpTimeout,
      _scanConfig.TargetTimeout,
      Info = new
      {
        Source = "utlib",
        client = new { _configuration.Name, _configuration.Version },
        Provider = "Some CI"
      }
    };

    var expectedContentString =
      await _messageSerializer.SerializeJsonContent(expectedPayload).ReadAsStringAsync();

    _commandDispatcher.Execute(Arg.Any<CreateScan>())
      .Returns(Task.FromResult<Identifiable<string>?>(new Identifiable<string>("id")));

    // act 
    await _sut.CreateScan(_scanConfig);

    // assert
    await _commandDispatcher.Received(1)
      .Execute(Arg.Is<CreateScan>(x => x.Url.EndsWith("/api/v1/scans") && x.Method == HttpMethod.Post &&
                                       x.ExpectReply &&
                                       expectedContentString == x.Body.GetSync(y => y.ReadAsStringAsync())));
  }

  [Fact]
  public async Task CreateScan_WhenResultIsNull_ThrowError()
  {
    // arrange
    _commandDispatcher.Execute(Arg.Any<CreateScan>())
      .Returns(Task.FromResult<Identifiable<string>?>(null));

    // act 
    var act = () => _sut.CreateScan(_scanConfig);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }

  [Fact]
  public async Task ListIssues_ReturnAListOfIssues()
  {
    // arrange
    var issues = new List<Issue>
    {
      new(IssueId,
        "Cross-site request forgery is a type of malicious website exploit.",
        "Database connection crashed",
        "The best way to protect against those kind of issues is making sure the Database resources are sufficient",
        new Request("https://brokencrystals.com/") { Method = HttpMethod.Get },
        new Request("https://brokencrystals.com/") { Method = HttpMethod.Get },
        $"{_configuration.Api}/scans/{ScanId}/issues/{IssueId}",
        1,
        Severity.Medium,
        Protocol.Http,
        DateTime.UtcNow) { Cvss = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:N/I:N/A:L" }
    };

    _commandDispatcher.Execute(Arg.Any<ListIssues>())
      .Returns(Task.FromResult<IEnumerable<Issue>?>(issues));

    // act
    var result = await _sut.ListIssues(ScanId);

    // assert
    result.Should().BeEquivalentTo(issues);
    await _commandDispatcher.Received(1)
      .Execute(Arg.Is<ListIssues>(x =>
        x.Url.EndsWith($"/api/v1/scans/{ScanId}/issues") && x.Method == HttpMethod.Get && x.ExpectReply));
  }

  [Fact]
  public async Task ListIssues_WhenResultIsNull_ThrowError()
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
      .Execute(Arg.Is<StopScan>(x => x.Url.EndsWith($"/api/v1/scans/{ScanId}/stop") && x.Method == HttpMethod.Get));
  }

  [Fact]
  public async Task DeleteScan_DeletesScan()
  {
    // act
    await _sut.DeleteScan(ScanId);

    // assert
    await _commandDispatcher.Received(1)
      .Execute(Arg.Is<DeleteScan>(x => x.Url.EndsWith($"/api/v1/scans/{ScanId}/delete") && x.Method == HttpMethod.Get));
  }

  [Fact]
  public async Task GetScan_ReturnsScanState()
  {
    // arrange
    var scanState = new ScanState(ScanStatus.Done);

    _commandDispatcher.Execute(Arg.Any<GetScan>())
      .Returns(Task.FromResult<ScanState?>(scanState));

    // act
    var result = await _sut.GetScan(ScanId);

    // assert
    result.Should().Be(scanState);
    await _commandDispatcher.Received(1)
      .Execute(Arg.Is<GetScan>(x =>
        x.Url.EndsWith($"/api/v1/scans/{ScanId}") && x.Method == HttpMethod.Get && x.ExpectReply));
  }

  [Fact]
  public async Task GetScan_WhenResultIsNull_ThrowError()
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
    var options = new UploadHarOptions(new Har(), "filename.har");

    _commandDispatcher.Execute(Arg.Any<UploadHar>())
      .Returns(Task.FromResult<Identifiable<string>?>(new Identifiable<string>(HarId)));

    // act
    var result = await _sut.UploadHar(options);

    // assert
    result.Should().Be(HarId);
    await _commandDispatcher.Received(1)
      .Execute(Arg.Is<UploadHar>(x =>
        x.Url.EndsWith($"/api/v1/files") && x.Method == HttpMethod.Post && x.ExpectReply && (
          x.Params == null || x.Params.All(y => y.Key != "discard"))));
  }

  [Fact]
  public async Task UploadHar_WithDiscardOption_CreatesNewHar()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har", true);

    _commandDispatcher.Execute(Arg.Any<UploadHar>())
      .Returns(Task.FromResult<Identifiable<string>?>(new Identifiable<string>(HarId)));

    // act
    var result = await _sut.UploadHar(options);

    // assert
    result.Should().Be(HarId);
    await _commandDispatcher.Received(1)
      .Execute(Arg.Is<UploadHar>(x =>
        x.Url.EndsWith($"/api/v1/files") && x.Method == HttpMethod.Post && x.Params != null &&
        x.Params.Any(y => y.Key == "discard" && y.Value == "true")));
  }

  [Fact]
  public async Task UploadHar_PassesHarInfoInThePayload()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har", true);

    var jsonFileContent = _messageSerializer.SerializeJsonContent(options.Har);
    var fileNameContent = new StringContent(options.FileName);

    _commandDispatcher.Execute(Arg.Any<UploadHar>())
      .Returns(Task.FromResult<Identifiable<string>?>(new Identifiable<string>(HarId)));

    Func<HttpContent?, bool> uploadHarContentMatcher = httpContent =>
    {
      var quotedBoundary = httpContent?.Headers.ContentType?
        .Parameters?.FirstOrDefault(x => x.Name == "boundary")?.Value;

      var originalBoundary = quotedBoundary is not null
        ? quotedBoundary.Trim('\"')
        : Guid.NewGuid().ToString();

      var testContent = new MultipartFormDataContent(originalBoundary);
      testContent.Add(jsonFileContent, "file");
      testContent.Add(fileNameContent, "filename");

      var testContentAsString = testContent.GetSync(x => x.ReadAsStringAsync());
      var contentAsString = httpContent.GetSync(x => x.ReadAsStringAsync());

      return contentAsString is not null && testContentAsString is not null &&
             contentAsString == testContentAsString;
    };

    // act
    await _sut.UploadHar(options);

    // assert
    await _commandDispatcher.Received(1)
      .Execute(Arg.Is<UploadHar>(x => uploadHarContentMatcher(x.Body)));
  }

  [Fact]
  public async Task UploadHar_WhenResultIsNull_ThrowError()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har");

    _commandDispatcher.Execute(Arg.Any<UploadHar>())
      .Returns(Task.FromResult<Identifiable<string>?>(null));

    // act 
    var act = () => _sut.UploadHar(options);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }
}
