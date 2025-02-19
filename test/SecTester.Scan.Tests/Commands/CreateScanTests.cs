using SecTester.Scan.Tests.Extensions;

namespace SecTester.Scan.Tests.Commands;

public class CreateScanTests
{
  private const string ScanName = "Scan Name";
  private const string ProjectId = "e9a2eX46EkidKhn3uqdYvE";
  private const string RepeaterId = "g5MvgM74sweGcK1U6hvs76";
  private const string FileId = "6aJa25Yd8DdXEcZg3QFoi8";

  private readonly ScanConfig _scanConfig = new(ScanName)
  {
    Module = Module.Dast,
    Repeaters = new[]
    {
      RepeaterId
    },
    Smart = true,
    Tests = new[]
    {
      TestType.CrossSiteRequestForgery, TestType.BrokenJwtAuthentication
    },
    DiscoveryTypes = new[]
    {
      Discovery.Crawler
    },
    FileId = FileId,
    HostsFilter = new[]
    {
      "example.com"
    },
    PoolSize = 2,
    ProjectId = ProjectId,
    TargetTimeout = 10,
    AttackParamLocations = new[]
    {
      AttackParamLocation.Body, AttackParamLocation.Header
    },
    SkipStaticParams = true,
    SlowEpTimeout = 20
  };

  [Fact]
  public void Constructor_ConstructsInstance()
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
        client = new
        {
          Name = "Configuration Name",
          Version = "Configuration Version"
        },
        Provider = "Some CI"
      }
    };

    // act 
    var command = new CreateScan(_scanConfig, "Configuration Name", "Configuration Version", "Some CI");

    // assert
    command.Should()
      .BeEquivalentTo(
        new
        {
          Url = "/api/v1/scans",
          Method = HttpMethod.Post,
          Params = default(IEnumerable<KeyValuePair<string, string>>?),
          ExpectReply = true,
          Body = new StringContent(MessageSerializer.Serialize(expectedPayload), Encoding.UTF8, "application/json")
        }, config => config.IncludingNestedObjects()
          .Using<StringContent>(ctx =>
          {
            ctx.Subject.ReadHttpContentAsString().Should().Be(ctx.Expectation.ReadHttpContentAsString());
            ctx.Subject.Headers.ContentType.Should().Be(ctx.Expectation.Headers.ContentType);
          })
          .When(info => info.Path.EndsWith(nameof(CreateScan.Body)))
      );
  }
}
