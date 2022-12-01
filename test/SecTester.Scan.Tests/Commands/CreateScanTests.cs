namespace SecTester.Scan.Tests.Commands;

public class CreateScanTests
{
  [Fact]
  public void Constructor_ConstructsInstance()
  {
    // arrange
    var expectedPayload = new
    {
      ScanFixture.ScanConfig.Name,
      ScanFixture.ScanConfig.Module,
      ScanFixture.ScanConfig.Tests,
      ScanFixture.ScanConfig.DiscoveryTypes,
      ScanFixture.ScanConfig.PoolSize,
      ScanFixture.ScanConfig.AttackParamLocations,
      ScanFixture.ScanConfig.FileId,
      ScanFixture.ScanConfig.HostsFilter,
      ScanFixture.ScanConfig.Repeaters,
      ScanFixture.ScanConfig.Smart,
      ScanFixture.ScanConfig.SkipStaticParams,
      ScanFixture.ScanConfig.ProjectId,
      ScanFixture.ScanConfig.SlowEpTimeout,
      ScanFixture.ScanConfig.TargetTimeout,
      Info = new
      {
        Source = "utlib",
        client = new { Name = "Configuration Name", Version = "Configuration Version" },
        Provider = "Some CI"
      }
    };

    // act 
    var command = new CreateScan(ScanFixture.ScanConfig, "Configuration Name", "Configuration Version", "Some CI");

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
