using SecTester.Core.Exceptions;
using SecTester.Scan.Commands;
using SecTester.Scan.Tests.Fixtures;

namespace SecTester.Scan.Tests;

public class DefaultScansTests : ScanFixture
{
  private const string NullResultMessage = "Something went wrong. Please try again later.";
  private static readonly StringContent _content = new("");

  private readonly Scans _sut;

  public DefaultScansTests()
  {
    _sut = new DefaultScans(Configuration, CommandDispatcher, HttpContentFactory, CiDiscovery);
  }

  [Fact]
  public async Task CreateScan_CreatesNewScan()
  {
    // arrange
    CommandDispatcher.Execute(Arg.Any<CreateScan>())
      .Returns(Task.FromResult<Identifiable<string>?>(new Identifiable<string>(ScanId)));

    HttpContentFactory.CreateJsonContent(Arg.Any<object>()).Returns(_content);

    // act 
    var result = await _sut.CreateScan(ScanConfig).ConfigureAwait(false);

    // assert
    result.Should().Be(ScanId);
    await CommandDispatcher.Received(1)
      .Execute(Arg.Is<CreateScan>(x => x.Body == _content)).ConfigureAwait(false);
  }

  [Fact]
  public async Task CreateScan_ResultIsNull_ThrowError()
  {
    // arrange
    CommandDispatcher.Execute(Arg.Any<CreateScan>())
      .Returns(Task.FromResult<Identifiable<string>?>(null));

    // act 
    var act = () => _sut.CreateScan(ScanConfig);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage).ConfigureAwait(false);
  }

  [Fact]
  public async Task ListIssues_ReturnListOfIssues()
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
        $"{Configuration.Api}/scans/{ScanId}/issues/{IssueId}",
        1,
        Severity.Medium,
        Protocol.Http,
        DateTime.UtcNow) { Cvss = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:N/I:N/A:L" }
    };

    CommandDispatcher.Execute(Arg.Any<ListIssues>())
      .Returns(Task.FromResult<IEnumerable<Issue>?>(issues));

    // act
    var result = await _sut.ListIssues(ScanId).ConfigureAwait(false);

    // assert
    result.Should().BeEquivalentTo(issues);
    await CommandDispatcher.Received(1)
      .Execute(Arg.Is<ListIssues>(x =>
        x.Url.Contains(ScanId))).ConfigureAwait(false);
  }

  [Fact]
  public async Task ListIssues_ResultIsNull_ThrowError()
  {
    // arrange
    CommandDispatcher.Execute(Arg.Any<ListIssues>())
      .Returns(Task.FromResult<IEnumerable<Issue>?>(null));

    // act 
    var act = () => _sut.ListIssues(ScanId);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage).ConfigureAwait(false);
  }

  [Fact]
  public async Task StopScan_StopsScan()
  {
    // act
    await _sut.StopScan(ScanId).ConfigureAwait(false);

    // assert
    await CommandDispatcher.Received(1)
      .Execute(Arg.Is<StopScan>(x =>
        x.Url.Contains(ScanId))).ConfigureAwait(false);
  }

  [Fact]
  public async Task DeleteScan_DeletesScan()
  {
    // act
    await _sut.DeleteScan(ScanId).ConfigureAwait(false);

    // assert
    await CommandDispatcher.Received(1)
      .Execute(Arg.Is<DeleteScan>(x =>
        x.Url.Contains(ScanId))).ConfigureAwait(false);
  }

  [Fact]
  public async Task GetScan_ReturnsScanState()
  {
    // arrange
    var scanState = new ScanState(ScanStatus.Done);

    CommandDispatcher.Execute(Arg.Any<GetScan>())
      .Returns(Task.FromResult<ScanState?>(scanState));

    // act
    var result = await _sut.GetScan(ScanId).ConfigureAwait(false);

    // assert
    result.Should().Be(scanState);
    await CommandDispatcher.Received(1)
      .Execute(Arg.Is<GetScan>(x =>
        x.Url.Contains(ScanId))).ConfigureAwait(false);
  }

  [Fact]
  public async Task GetScan_ResultIsNull_ThrowError()
  {
    // arrange
    CommandDispatcher.Execute(Arg.Any<GetScan>())
      .Returns(Task.FromResult<ScanState?>(null));

    // act 
    var act = () => _sut.GetScan(ScanId);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage).ConfigureAwait(false);
  }

  [Fact]
  public async Task UploadHar_CreatesNewHar()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har");

    CommandDispatcher.Execute(Arg.Any<UploadHar>())
      .Returns(Task.FromResult<Identifiable<string>?>(new Identifiable<string>(HarId)));

    HttpContentFactory.CreateHarContent(options).Returns(_content);

    // act
    var result = await _sut.UploadHar(options).ConfigureAwait(false);

    // assert
    result.Should().Be(HarId);
    await CommandDispatcher.Received(1)
      .Execute(Arg.Is<UploadHar>(x => x.Body == _content)).ConfigureAwait(false);
  }

  [Fact]
  public async Task UploadHar_ResultIsNull_ThrowError()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har");

    CommandDispatcher.Execute(Arg.Any<UploadHar>())
      .Returns(Task.FromResult<Identifiable<string>?>(null));

    // act 
    var act = () => _sut.UploadHar(options);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage).ConfigureAwait(false);
  }
}
