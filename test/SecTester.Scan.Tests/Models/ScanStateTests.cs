using SecTester.Scan.Models;

namespace SecTester.Scan.Tests.Models;

public class ScanStateTests
{
  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // arrange
    const ScanStatus status = ScanStatus.Done;
    const int entryPoints = 1;
    const int totalParams = 2;
    const int requests = 3;
    const int elapsed = 4;
    const bool discovering = true;
    var startTime = DateTime.Now;
    var endTime = DateTime.Now;
    var createdAt = DateTime.Now;
    var issuesBySeverity = new IssueGroup[] { };

    // act
    var scanState = new ScanState(status, issuesBySeverity, entryPoints,
      totalParams, discovering, requests, elapsed,
      startTime, endTime, createdAt);

    // assert
    scanState.Status.Should().Be(status);
    scanState.EntryPoints.Should().Be(entryPoints);
    scanState.TotalParams.Should().Be(totalParams);
    scanState.Requests.Should().Be(requests);
    scanState.Elapsed.Should().Be(elapsed);
    scanState.Discovering.Should().Be(discovering);
    scanState.StartTime.Should().Be(startTime);
    scanState.EndTime.Should().Be(endTime);
    scanState.CreatedAt.Should().Be(createdAt);
    scanState.IssuesBySeverity.Should().BeEquivalentTo(issuesBySeverity);
  }
}
