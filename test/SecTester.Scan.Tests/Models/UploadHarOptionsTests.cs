namespace SecTester.Scan.Tests.Models;

public class UploadHarOptionsTests
{
  private const string HarFileName = "file.har";

  private readonly Har _har = new(
    new Log(
      new Tool("name", "v1.1.1")
    )
  );

  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // act
    var options = new UploadHarOptions(_har, HarFileName, true);

    // assert
    options.Should().BeEquivalentTo(new
    {
      FileName = HarFileName,
      Har = _har,
      Discard = true
    });
  }

  [Fact]
  public void Constructor_GivenNullHarContent_ThrowError()
  {
    // act
    var act = () => new UploadHarOptions(null!, HarFileName);

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
