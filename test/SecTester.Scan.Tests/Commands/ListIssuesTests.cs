using SecTester.Scan.Commands;
using SecTester.Scan.Tests.Fixtures;

namespace SecTester.Scan.Tests.Commands;

public class ListIssuesTests : ScanFixture
{
  [Fact]
  public void Constructor_ConstructsInstance()
  {
    // act 
    var command = new ListIssues(ScanId);

    // assert
    command.Url.Should().Be($"/api/v1/scans/{ScanId}/issues");
    command.Method.Should().Be(HttpMethod.Get);
    command.ExpectReply.Should().BeTrue();
  }
}
