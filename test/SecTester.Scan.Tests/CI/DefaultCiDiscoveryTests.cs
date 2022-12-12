namespace SecTester.Scan.Tests.CI;

public class DefaultCiDiscoveryTests
{
  [Fact]
  public void Constructor_GivenEmptyEnvironment_CreatesInstanceWithDefaultValues()
  {
    // act
    var sut = new DefaultCiDiscovery(new Dictionary<string, string>());

    // assert
    sut.Should().BeEquivalentTo(new
    {
      Server = null as CiServer,
      IsCi = false,
      IsPr = false
    });
  }

  [Fact]
  public void Constructor_GivenEnvironmentWithCI_CreatesInstance()
  {
    // act
    var sut = new DefaultCiDiscovery(new Dictionary<string, string>() { { "GITHUB_ACTIONS", "" } });

    // assert
    sut.Should().BeEquivalentTo(new
    {
      Server = CiServer.GithubActions,
      IsCi = true,
      IsPr = false
    });
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
    sut.Should().BeEquivalentTo(new
    {
      Server = CiServer.GithubActions,
      IsCi = true,
      IsPr = true
    });
  }
}
