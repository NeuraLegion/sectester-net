using SecTester.Scan.CI;

namespace SecTester.Scan.Tests.CI;

public class DefaultCiDiscoveryTests
{
  private readonly DefaultCiDiscovery _sut = new ();

  [Fact]
  public void Constructor_InitializesAsUnknownServer()
  {
    // assert
    _sut.Server.Should().Be(CiServer.Unknown);
  }   
  
  [Fact]
  public void IsCi_WithUnknownServer_ReturnsFalse()
  {
    // assert
    _sut.IsCi.Should().BeFalse();
  }
  
  [Fact]
  public void IsPr_ReturnsFalse()
  {
    // assert
    _sut.IsPr.Should().BeFalse();
  }
}
