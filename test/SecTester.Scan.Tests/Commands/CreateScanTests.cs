using System.Text;
using SecTester.Bus.Dispatchers;
using SecTester.Scan.Commands;
using SecTester.Scan.Tests.Fixtures;

namespace SecTester.Scan.Tests.Commands;

public class CreateScanTests : ScanFixture
{
  [Fact]
  public void Constructor_ConstructsInstance()
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
      Info = new
      {
        Source = "utlib",
        client = new { Name = "Configuration Name", Version = "Configuration Version" },
        Provider = "Some CI"
      }
    };

    // act 
    var command = new CreateScan(ScanConfig, "Configuration Name", "Configuration Version", "Some CI");

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
            ReadHttpContentAsString(ctx.Subject).Should().Be(ReadHttpContentAsString(ctx.Expectation));
            ctx.Subject.Headers.ContentType.Should().Be(ctx.Expectation.Headers.ContentType);
          })
          .When(info => info.Path.EndsWith(nameof(CreateScan.Body)))
      );
  }
}
