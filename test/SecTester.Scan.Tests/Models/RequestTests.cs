#pragma warning disable CS8604

using SecTester.Scan.Models;

namespace SecTester.Scan.Tests.Models;

public class RequestTests
{
  private const string Url = "http://example.com/";
  private const string Body = "body";

  private static readonly Dictionary<string, string> Headers = new() { { "content-type", "application/json" } };

  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // act
    var request = new Request(Url, RequestMethod.Delete, Headers, Body, Protocol.Ws);

    // assert
    request.Url.Should().Be(Url);
    request.Body.Should().Be(Body);
    request.Method.Should().Be(RequestMethod.Delete);
    request.Protocol.Should().Be(Protocol.Ws);
    request.Headers.Should().BeEquivalentTo(Headers);
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
