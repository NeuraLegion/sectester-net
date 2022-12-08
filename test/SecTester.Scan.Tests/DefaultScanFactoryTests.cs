using SecTester.Scan.Tests.Mocks;

namespace SecTester.Scan.Tests;

public class DefaultScanFactoryTests : IDisposable
{
  private const string FileId = "6aJa25Yd8DdXEcZg3QFoi8";
  private const string ScanId = "roMq1UVuhPKkndLERNKnA8";

  private readonly Configuration _configuration = new("app.neuralegion.com");
  private readonly MockLogger _logger = Substitute.For<MockLogger>();
  private readonly ScanSettingsOptions _options = Substitute.For<ScanSettingsOptions>();
  private readonly Scans _scans = Substitute.For<Scans>();
  private readonly ScanFactory _sut;
  private readonly SystemTimeProvider _systemTimeProvider = Substitute.For<SystemTimeProvider>();

  public DefaultScanFactoryTests()
  {
    _sut = new DefaultScanFactory(_configuration, _scans, _logger, _systemTimeProvider);
  }

  public void Dispose()
  {
    _options.ClearSubstitute();
    _scans.ClearSubstitute();
    _systemTimeProvider.ClearSubstitute();
    _logger.ClearSubstitute();

    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task CreateScan_CreatesScan()
  {
    // arrange
    _options.Name.ReturnsForAnyArgs(null as string);
    _options.AttackParamLocations.ReturnsForAnyArgs(null as IEnumerable<AttackParamLocation>);
    _options.Target.Returns(new Target("https://example.com"));
    _options.Tests.Returns(new List<TestType>
    {
      TestType.DomXss
    });

    _scans.UploadHar(Arg.Any<UploadHarOptions>()).Returns(FileId);
    _scans.CreateScan(Arg.Any<ScanConfig>()).Returns(ScanId);

    // act
    var result = await _sut.CreateScan(_options);

    // assert
    result.Id.Should().Be(ScanId);
    await _scans.Received(1).CreateScan(Arg.Is<ScanConfig>(x =>
      x.Name == "GET example.com" &&
      x.FileId == FileId &&
      x.Module == Module.Dast &&
      x.Tests!.Contains(TestType.DomXss) &&
      x.Tests!.Count() == 1 &&
      x.DiscoveryTypes!.Contains(Discovery.Archive) &&
      x.DiscoveryTypes!.Count() == 1
    ));
  }

  [Fact]
  public async Task CreateScan_GeneratesUploadHarFile()
  {
    // arrange
    _options.Name.ReturnsForAnyArgs(null as string);
    _options.AttackParamLocations.ReturnsForAnyArgs(null as IEnumerable<AttackParamLocation>);
    _options.Target.Returns(new Target("https://example.com"));
    _options.Tests.Returns(new List<TestType>
    {
      TestType.DomXss
    });

    _scans.UploadHar(Arg.Any<UploadHarOptions>()).Returns(FileId);
    _scans.CreateScan(Arg.Any<ScanConfig>()).Returns(ScanId);

    // act
    await _sut.CreateScan(_options);

    // assert
    await _scans.Received(1).UploadHar(Arg.Is<UploadHarOptions>(x =>
      x.Discard &&
      Regex.IsMatch(x.FileName, @"^example\.com-[a-z\d-]+\.har$") &&
      x.Har.Log.Creator.Version == _configuration.Version &&
      x.Har.Log.Creator.Name == _configuration.Name &&
      x.Har.Log.Version == "1.2"
    ));
  }

  [Fact]
  public async Task CreateScan_TruncatesHarFilename()
  {
    // arrange
    _options.Name.ReturnsForAnyArgs(null as string);
    _options.AttackParamLocations.ReturnsForAnyArgs(null as IEnumerable<AttackParamLocation>);
    _options.Target.Returns(
      new Target($"https://{new string('a', 1 + DefaultScanFactory.MaxSlugLength)}.example.com"));
    _options.Tests.Returns(new List<TestType>
    {
      TestType.DomXss
    });

    _scans.UploadHar(Arg.Any<UploadHarOptions>()).Returns(FileId);
    _scans.CreateScan(Arg.Any<ScanConfig>()).Returns(ScanId);

    // act
    await _sut.CreateScan(_options);

    // assert
    await _scans.Received(1).UploadHar(Arg.Is<UploadHarOptions>(x =>
      Regex.IsMatch(x.FileName, $"^.{{{DefaultScanFactory.MaxSlugLength}}}-[a-z\\d-]+\\.har$")));
  }
}
