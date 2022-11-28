namespace SecTester.Scan.Tests;

public class ScanTests
{
  [Fact]
  public void Constructor_GivenNullScans_ThrowError()
  {
    // act
    var act = () => new Scan(null!, new ScanOptions());

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*scans*");
  }

  [Fact]
  public void Constructor_GivenNullScanOptions_ThrowError()
  {
    // arrange
    var scans = Substitute.For<Scans>();

    // act
    var act = () => new Scan(scans, null!);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*options*");
  }

  [Fact]
  public void Active_DefaultValue_ReturnTrue()
  {
    // arrange
    var scans = Substitute.For<Scans>();

    // act
    using var scan = new Scan(scans, new ScanOptions());

    // assert
    scan.Active.Should().BeTrue();
  }

  [Fact]
  public void Done_DefaultValue_ReturnTrue()
  {
    // arrange
    var scans = Substitute.For<Scans>();

    // act
    using var scan = new Scan(scans, new ScanOptions());

    // assert
    scan.Done.Should().BeFalse();
  }
}
