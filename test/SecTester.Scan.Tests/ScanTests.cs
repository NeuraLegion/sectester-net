#pragma warning disable CS8604

namespace SecTester.Scan.Tests;

public class ScanTests
{
  [Fact]
  public void Constructor_GivenNullScans_ThrowError()
  {
    // act
    Action act = () => new Scan(null as Scans, 0, 0);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Active_DefaultValue_ReturnTrue()
  {
    // arrange
    var scans = Substitute.For<Scans>();

    // act
    using var scan = new Scan(scans, 0, 0);

    // assert
    scan.Active.Should().BeTrue();
  }

  [Fact]
  public void Done_DefaultValue_ReturnTrue()
  {
    // arrange
    var scans = Substitute.For<Scans>();

    // act
    using var scan = new Scan(scans, 0, 0);

    // assert
    scan.Done.Should().BeFalse();
  }
}
