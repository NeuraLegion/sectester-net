using SecTester.Core.Extensions;

namespace SecTester.Core.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
  [Fact]
  public void AddSecTesterConfig_Hostname_ReturnInstanceWithDefaultOptions()
  {
    // arrange
    const string hostname = "app.neuralegion.com";
    var services = new ServiceCollection();

    // act
    services.AddSecTesterConfig(hostname);

    // assert
    var provider = services.BuildServiceProvider();
    var restService = provider.GetRequiredService<Configuration>();
    restService.Should().BeOfType<Configuration>();
  }

  [Fact]
  public void AddSecTesterConfig_GivenConfig_ReturnInstance()
  {
    // arrange
    var configuration = new Configuration("app.neuralegion.com");
    var services = new ServiceCollection();

    // act
    services.AddSecTesterConfig(configuration);

    // assert
    var provider = services.BuildServiceProvider();
    var restService = provider.GetRequiredService<Configuration>();
    restService.Should().BeEquivalentTo(configuration);
  }
}
