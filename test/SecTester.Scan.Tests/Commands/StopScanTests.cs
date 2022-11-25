using SecTester.Scan.Commands;
using SecTester.Scan.Tests.Fixtures;

namespace SecTester.Scan.Tests.Commands;

public class StopScanTests : ScanFixture
{
  [Fact]
  public void Constructor_ConstructsInstance()
  {
    // act 
    var command = new StopScan(ScanId);

    // assert
    command.Url.Should().Be($"/api/v1/scans/{ScanId}/stop");
    command.Method.Should().Be(HttpMethod.Get);
    command.ExpectReply.Should().BeFalse();
  }
}
