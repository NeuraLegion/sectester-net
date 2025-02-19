namespace SecTester.Scan.Tests;

public class DefaultScanFactoryTests : IDisposable
{
  private const string FileId = "6aJa25Yd8DdXEcZg3QFoi8";
  private const string ScanId = "roMq1UVuhPKkndLERNKnA8";

  private readonly Configuration _configuration = new("app.brightsec.com");
  private readonly ILoggerFactory _loggerFactory = Substitute.For<ILoggerFactory>();
  private readonly IScans _scans = Substitute.For<IScans>();
  private readonly ISystemTimeProvider _systemTimeProvider = Substitute.For<ISystemTimeProvider>();
  private readonly IScanFactory _sut;

  public DefaultScanFactoryTests()
  {
    _sut = new DefaultScanFactory(_configuration, _scans, _loggerFactory, _systemTimeProvider);
  }

  public void Dispose()
  {
    _scans.ClearSubstitute();
    _systemTimeProvider.ClearSubstitute();
    _loggerFactory.ClearSubstitute();

    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task CreateScan_CreatesScan()
  {
    // arrange
    var settings = new ScanSettings("MyScan", new Target("https://example.com"), new List<TestType>
    {
      TestType.CrossSiteScripting
    });
    _scans.UploadHar(Arg.Any<UploadHarOptions>()).Returns(FileId);
    _scans.CreateScan(Arg.Any<ScanConfig>()).Returns(ScanId);

    // act
    var result = await _sut.CreateScan(settings);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Id = ScanId
    });
    await _scans.Received(1).CreateScan(Arg.Is<ScanConfig>(x =>
      x.Name == "MyScan" &&
      x.FileId == FileId &&
      x.Module == Module.Dast &&
      x.Tests!.Contains(TestType.CrossSiteScripting) &&
      x.Tests!.Count() == 1 &&
      x.DiscoveryTypes!.Contains(Discovery.Archive) &&
      x.DiscoveryTypes!.Count() == 1
    ));
  }

  [Fact]
  public async Task CreateScan_GeneratesUploadHarFile()
  {
    // arrange
    var settings = new ScanSettings("MyScan", new Target("https://example.com"), new List<TestType>
    {
      TestType.CrossSiteScripting
    });

    _scans.UploadHar(Arg.Any<UploadHarOptions>()).Returns(FileId);
    _scans.CreateScan(Arg.Any<ScanConfig>()).Returns(ScanId);

    // act
    await _sut.CreateScan(settings);

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
    var settings = new ScanSettings("MyScan", new Target($"https://{new string('a', 1 + DefaultScanFactory.MaxSlugLength)}.example.com"),
      new List<TestType>
      {
        TestType.CrossSiteScripting
      });

    _scans.UploadHar(Arg.Any<UploadHarOptions>()).Returns(FileId);
    _scans.CreateScan(Arg.Any<ScanConfig>()).Returns(ScanId);

    // act
    await _sut.CreateScan(settings);

    // assert
    await _scans.Received(1).UploadHar(Arg.Is<UploadHarOptions>(x =>
      Regex.IsMatch(x.FileName, $"^.{{{DefaultScanFactory.MaxSlugLength}}}-[a-z\\d-]+\\.har$")));
  }
}
