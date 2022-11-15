#pragma warning disable CS8604

using SecTester.Scan.Models;

namespace SecTester.Scan.Tests.Models;

public class ResponseTests
{
  private const string Body = "body";
  private static readonly Dictionary<string, string> Headers = new() { { "content-type", "application/json" } };

  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // act
    var response = new Response(Protocol.Ws, 200, Headers, Body);

    // assert
    response.Status.Should().Be(200);
    response.Body.Should().Be(Body);
    response.Protocol.Should().Be(Protocol.Ws);
    response.Headers.Should().BeEquivalentTo(Headers);
  }

  [Fact]
  public void Constructor_GivenNullUrl_ThrowError()
  {
    // act
    Action act = () => new Request(null as string);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }
}
