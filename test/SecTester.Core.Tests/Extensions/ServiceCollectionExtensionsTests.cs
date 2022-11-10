using SecTester.Core.Extensions;

namespace SecTester.Core.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
  private readonly ServiceCollection _services;

  public ServiceCollectionExtensionsTests()
  {
    _services = new ServiceCollection();
  }

  [Fact]
  public void AddSecTesterConfig_Hostname_ReturnInstanceWithDefaultOptions()
  {
    // arrange
    const string hostname = "app.neuralegion.com";

    // act
    _services.AddSecTesterConfig(hostname);

    // assert
    using var provider = _services.BuildServiceProvider();
    var restService = provider.GetRequiredService<Configuration>();
    restService.Should().BeOfType<Configuration>();
  }

  [Fact]
  public void AddSecTesterConfig_GivenConfig_ReturnInstance()
  {
    // arrange
    var configuration = new Configuration("app.neuralegion.com");

    // act
    _services.AddSecTesterConfig(configuration);

    // assert
    using var provider = _services.BuildServiceProvider();
    var restService = provider.GetRequiredService<Configuration>();
    restService.Should().BeEquivalentTo(configuration);
  }
}
