using SecTester.Scan.Models;

namespace SecTester.Scan.Tests.Models;

public class IssueGroupTests
{
  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // act
    var issueGroup = new IssueGroup(2, Severity.High);

    // assert
    issueGroup.Number.Should().Be(2);
    issueGroup.Type.Should().Be(Severity.High);
  }
}
