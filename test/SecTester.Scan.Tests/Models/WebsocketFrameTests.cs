#pragma warning disable CS8604

using SecTester.Scan.Models;

namespace SecTester.Scan.Tests.Models;

public class WebsocketFrameTests
{
  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // act
    var websocketFrame = new WebsocketFrame(Frame.Incoming, 200, "data", 100);

    // assert
    websocketFrame.Type.Should().Be(Frame.Incoming);
    websocketFrame.Status.Should().Be(200);
    websocketFrame.Data.Should().Be("data");
    websocketFrame.Timestamp.Should().Be(100);
  }
}
