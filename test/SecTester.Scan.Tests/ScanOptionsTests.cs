namespace SecTester.Scan.Tests;

public class ScanOptionsTests
{
  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // act
    var scanOptions = new ScanOptions(1, 2);

    // assert
    scanOptions.PollingInterval.Should().Be(2);
    scanOptions.Timeout.Should().Be(1);
  }
}
