using Microsoft.Extensions.Logging;
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
  
  [Fact]
  public void AddLogging_SetMinimumLevelToErrorByDefault()
  {
    // arrange
    var services = new ServiceCollection();
    services.AddSystemTimeProvider();

    // act
    services.AddDefaultLogging();

    // assert
    var provider = services.BuildServiceProvider();
    var logger = provider.GetRequiredService<ILogger<ServiceCollectionExtensionsTests>>();
    logger.IsEnabled(LogLevel.Error).Should().BeTrue();
    logger.IsEnabled(LogLevel.Warning).Should().BeFalse();
  }
  
    
  [Fact]
  public void AddLogging_GivenLogLevel_SetMinimumLevelToValue()
  {
    // arrange
    var services = new ServiceCollection();
    services.AddSystemTimeProvider();

    // act
    services.AddDefaultLogging(LogLevel.Information);

    // assert
    var provider = services.BuildServiceProvider();
    var logger = provider.GetRequiredService<ILogger<ServiceCollectionExtensionsTests>>();
    logger.IsEnabled(LogLevel.Information).Should().BeTrue();
    logger.IsEnabled(LogLevel.Debug).Should().BeFalse();
  }
}
