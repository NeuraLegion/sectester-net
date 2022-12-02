using SecTester.Scan.Models;
using SecTester.Scan.Target.Har;

namespace SecTester.Scan.Tests.Models;

public class UploadHarContentOptionsTests
{
  private const string FileName = "file.har";
  private readonly Har _har = new();

  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // act
    var options = new UploadHarOptions(_har, FileName, true);

    // assert
    options.Should().BeEquivalentTo(new
    {
      Har = _har,
      FileName,
      Discard = true
    });
  }

  [Fact]
  public void Constructor_GivenNullHarContent_ThrowError()
  {
    // act
    var act = () => new UploadHarOptions(null!, FileName);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*Har*");
  }

  [Fact]
  public void Constructor_GivenNullFileName_ThrowError()
  {
    // act
    var act = () => new UploadHarOptions(_har, null!);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*Filename*");
  }
}
