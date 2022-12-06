namespace SecTester.Scan.Tests.Models;

public class UploadHarOptionsTests
{
  private const string HarFileName = "file.har";

  private static readonly Har Har = new(
    new Log(
      new Tool("Configuration_Name", "Configuration_Version")
    )
  );

  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // act
    var options = new UploadHarOptions(Har, HarFileName, true);

    // assert
    options.Should().BeEquivalentTo(new { FileName = HarFileName, Har, Discard = true });
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
    var act = () => new UploadHarOptions(Har, null!);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*Filename*");
  }
}
