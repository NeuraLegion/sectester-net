using SecTester.Scan.Tests.Fixtures;

namespace SecTester.Scan.Tests;

public class DefaultScansTests : ScanFixture
{
  private const string NullResultMessage = "Something went wrong. Please try again later.";

  private readonly Scans _sut;

  public DefaultScansTests()
  {
    _sut = new DefaultScans(Configuration, CommandDispatcher, CiDiscovery);
  }

  [Fact]
  public async Task CreateScan_CreatesNewScan()
  {
    // arrange
    CommandDispatcher.Execute(Arg.Any<CreateScan>())
      .Returns(Task.FromResult<Identifiable<string>?>(new Identifiable<string>(ScanId)));

    // act 
    var result = await _sut.CreateScan(ScanConfig);

    // assert
    result.Should().Be(ScanId);
    await CommandDispatcher.Received(1)
      .Execute(Arg.Any<CreateScan>());
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
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }

  [Fact]
  public async Task ListIssues_ReturnListOfIssues()
  {
    // arrange
    CommandDispatcher.Execute(Arg.Any<ListIssues>())
      .Returns(Task.FromResult<IEnumerable<Issue>?>(Issues));

    // act
    var result = await _sut.ListIssues(ScanId);

    // assert
    result.Should().BeEquivalentTo(Issues);
    await CommandDispatcher.Received(1)
      .Execute(Arg.Any<ListIssues>());
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
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }

  [Fact]
  public async Task StopScan_StopsScan()
  {
    // act
    await _sut.StopScan(ScanId);

    // assert
    await CommandDispatcher.Received(1)
      .Execute(Arg.Any<StopScan>());
  }

  [Fact]
  public async Task DeleteScan_DeletesScan()
  {
    // act
    await _sut.DeleteScan(ScanId);

    // assert
    await CommandDispatcher.Received(1)
      .Execute(Arg.Any<DeleteScan>());
  }

  [Fact]
  public async Task GetScan_ReturnsScanState()
  {
    // arrange
    var scanState = new ScanState(ScanStatus.Done);

    CommandDispatcher.Execute(Arg.Any<GetScan>())
      .Returns(Task.FromResult<ScanState?>(scanState));

    // act
    var result = await _sut.GetScan(ScanId);

    // assert
    result.Should().Be(scanState);
    await CommandDispatcher.Received(1)
      .Execute(Arg.Any<GetScan>());
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
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }

  [Fact]
  public async Task UploadHar_CreatesNewHar()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har");

    CommandDispatcher.Execute(Arg.Any<UploadHar>())
      .Returns(Task.FromResult<Identifiable<string>?>(new Identifiable<string>(HarId)));

    // act
    var result = await _sut.UploadHar(options);

    // assert
    result.Should().Be(HarId);
    await CommandDispatcher.Received(1)
      .Execute(Arg.Any<UploadHar>());
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
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }
}
