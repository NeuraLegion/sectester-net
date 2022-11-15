#pragma warning disable CS8604

using SecTester.Scan.Models;

namespace SecTester.Scan.Tests.Models;

public class ScreenshotTests
{
  private const string Url = "http://example.com";
  private const string Title = "title";

  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // act
    var response = new Screenshot(Url, Title);

    // assert
    response.Url.Should().Be(Url);
    response.Title.Should().Be(Title);
  }

  [Fact]
  public void Constructor_GivenNullUrl_ThrowError()
  {
    // act
    Action act = () => new Screenshot(null as string, Title);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_GivenNullText_ThrowError()
  {
    // act
    Action act = () => new Screenshot(Url, null as string);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }
}
