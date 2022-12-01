namespace SecTester.Scan.Tests;

public class DefaultScansTests : IDisposable
{
  private const string NullResultMessage = "Something went wrong. Please try again later.";

  private readonly CommandDispatcher _commandDispatcher = Substitute.For<CommandDispatcher>();
  private readonly CiDiscovery _ciDiscovery = Substitute.For<CiDiscovery>();

  private readonly Scans _sut;

  public DefaultScansTests()
  {
    _sut = new DefaultScans(ScanFixture.Configuration, _commandDispatcher, _ciDiscovery);
  }

  public void Dispose()
  {
    _commandDispatcher.ClearSubstitute();
    _ciDiscovery.ClearSubstitute();

    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task CreateScan_CreatesNewScan()
  {
    // arrange
    _commandDispatcher.Execute(Arg.Any<CreateScan>())
      .Returns(new Identifiable<string>(ScanFixture.ScanId));

    // act 
    var result = await _sut.CreateScan(ScanFixture.ScanConfig);

    // assert
    result.Should().Be(ScanFixture.ScanId);
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
    var act = () => _sut.CreateScan(ScanFixture.ScanConfig);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }

  [Fact]
  public async Task ListIssues_ReturnListOfIssues()
  {
    // arrange
    _commandDispatcher.Execute(Arg.Any<ListIssues>())
      .Returns(ScanFixture.Issues);

    // act
    var result = await _sut.ListIssues(ScanFixture.ScanId);

    // assert
    result.Should().BeEquivalentTo(ScanFixture.Issues);
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
    var act = () => _sut.ListIssues(ScanFixture.ScanId);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }

  [Fact]
  public async Task StopScan_StopsScan()
  {
    // act
    await _sut.StopScan(ScanFixture.ScanId);

    // assert
    await _commandDispatcher.Received(1)
      .Execute(Arg.Any<StopScan>());
  }

  [Fact]
  public async Task DeleteScan_DeletesScan()
  {
    // act
    await _sut.DeleteScan(ScanFixture.ScanId);

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
    var result = await _sut.GetScan(ScanFixture.ScanId);

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
    var act = () => _sut.GetScan(ScanFixture.ScanId);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }

  [Fact]
  public async Task UploadHar_CreatesNewHar()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har");

    _commandDispatcher.Execute(Arg.Any<UploadHar>())
      .Returns(new Identifiable<string>(ScanFixture.HarId));

    // act
    var result = await _sut.UploadHar(options);

    // assert
    result.Should().Be(ScanFixture.HarId);
    await _commandDispatcher.Received(1)
      .Execute(Arg.Any<UploadHar>());
  }

  [Fact]
  public async Task UploadHar_ResultIsNull_ThrowError()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har");

    _commandDispatcher.Execute(Arg.Any<UploadHar>())
      .Returns(null as Identifiable<string>);

    // act 
    var act = () => _sut.UploadHar(options);

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage(NullResultMessage);
  }
}
