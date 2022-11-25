using SecTester.Bus.Dispatchers;
using SecTester.Scan.CI;
using SecTester.Scan.Commands;
using SecTester.Scan.Content;
using SecTester.Scan.Tests.Fixtures;

namespace SecTester.Scan.Tests.Commands;

public class CreateScanTests : ScanFixture
{
  private readonly DefaultHttpContentFactory _defaultHttpContentFactory =
    new(new DefaultMessageSerializer());

  [Fact]
  public async Task Constructor_ConstructsInstance()
  {
    // arrange
    var expectedPayload = new
    {
      ScanConfig.Name,
      ScanConfig.Module,
      ScanConfig.Tests,
      ScanConfig.DiscoveryTypes,
      ScanConfig.PoolSize,
      ScanConfig.AttackParamLocations,
      ScanConfig.FileId,
      ScanConfig.HostsFilter,
      ScanConfig.Repeaters,
      ScanConfig.Smart,
      ScanConfig.SkipStaticParams,
      ScanConfig.ProjectId,
      ScanConfig.SlowEpTimeout,
      ScanConfig.TargetTimeout,
      Info = new { Source = "utlib", client = new { Configuration.Name, Configuration.Version }, Provider = "Some CI" }
    };
    var expectedContent =
      await _defaultHttpContentFactory.CreateJsonContent(expectedPayload).ReadAsStringAsync().ConfigureAwait(false);

    CiDiscovery.Server.Returns(new CiServer("Some CI"));

    // act 
    var command = new CreateScan(ScanConfig, _defaultHttpContentFactory, CiDiscovery, Configuration);

    // assert
    var content = command.Body is null ? default : await command.Body.ReadAsStringAsync().ConfigureAwait(false);
    content.Should().Be(expectedContent);
    command.Url.Should().Be("/api/v1/scans");
    command.Method.Should().Be(HttpMethod.Post);
    command.ExpectReply.Should().BeTrue();
  }
}
