namespace SecTester.Scan.Tests.CI;

public class DefaultCiDiscoveryTests
{
  [Fact]
  public void Constructor_GivenEmptyEnvironment_CreatesInstanceWithDefaultValues()
  {
    // act
    var sut = new DefaultCiDiscovery(new Dictionary<string, string>());

    // assert
    sut.Server.Should().BeNull();
    sut.IsCi.Should().BeFalse();
    sut.IsPr.Should().BeFalse();
  }

  [Fact]
  public void Constructor_GivenEnvironmentWithCI_CreatesInstance()
  {
    // act
    var sut = new DefaultCiDiscovery(new Dictionary<string, string>() { { "GITHUB_ACTIONS", "" } });

    // assert
    sut.Server.Should().BeSameAs(CiServer.GITHUB_ACTIONS);
    sut.IsCi.Should().BeTrue();
    sut.IsPr.Should().BeFalse();
  }

  [Fact]
  public void Constructor_GivenEnvironmentWithCIAndPR_CreatesInstance()
  {
    // act
    var sut = new DefaultCiDiscovery(new Dictionary<string, string>()
    {
      { "GITHUB_ACTIONS", "" }, { "GITHUB_EVENT_NAME", "pull_request" }
    });

    // assert
    sut.Server.Should().BeSameAs(CiServer.GITHUB_ACTIONS);
    sut.IsCi.Should().BeTrue();
    sut.IsPr.Should().BeTrue();
  }
}
