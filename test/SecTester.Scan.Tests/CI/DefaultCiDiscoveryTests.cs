namespace SecTester.Scan.Tests.CI;

public class DefaultCiDiscoveryTests
{
  private readonly DefaultCiDiscovery _sut = new();

  [Fact]
  public void Constructor_InitializesServerWithNull()
  {
    // assert
    _sut.Server.Should().BeNull();
  }

  [Fact]
  public void IsCi_ServerIsNull_ReturnsFalse()
  {
    // assert
    _sut.IsCi.Should().BeFalse();
  }
}
