namespace SecTester.Core.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
  private readonly ServiceCollection _services;

  private readonly Configuration _config = new("app.brightsec.com",
    new Credentials("0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk"));

  public ServiceCollectionExtensionsTests()
  {
    _services = new ServiceCollection();
  }

  [Fact]
  public void AddSecTesterConfig_Hostname_ReturnInstanceWithDefaultOptions()
  {
    // arrange
    const string hostname = "app.brightsec.com";

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
    var configuration = new Configuration("app.brightsec.com");

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
    var configuration = new Configuration("app.brightsec.com");
    var services = new ServiceCollection();

    // act
    services.AddSecTesterConfig(configuration);

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
    var configuration = new Configuration("app.brightsec.com", logLevel: LogLevel.Information);
    var services = new ServiceCollection();

    // act
    services.AddSecTesterConfig(configuration);

    // assert
    var provider = services.BuildServiceProvider();
    var logger = provider.GetRequiredService<ILogger<ServiceCollectionExtensionsTests>>();
    logger.IsEnabled(LogLevel.Information).Should().BeTrue();
    logger.IsEnabled(LogLevel.Debug).Should().BeFalse();
  }


  [Fact]
  public void AddHttpCommandDispatcher_ReturnHttpCommandDispatcherWithDefaultOptions()
  {
    // arrange
    var services = new ServiceCollection();
    services.AddSecTesterConfig(_config);

    // act
    services.AddHttpCommandDispatcher();

    // assert
    using var provider = services.BuildServiceProvider();
    var result = provider.GetRequiredService<ICommandDispatcher>();
    result.Should().BeOfType<HttpCommandDispatcher>();
  }

  [Fact]
  public void AddHttpCommandDispatcher_ReturnHttpCommandDispatcherConfig()
  {
    // arrange
    var services = new ServiceCollection();
    services.AddSecTesterConfig(_config);

    // act
    services.AddHttpCommandDispatcher();


    // assert
    using var provider = services.BuildServiceProvider();
    var result = provider.GetRequiredService<HttpCommandDispatcherConfig>();
    result.Should().BeEquivalentTo(new
    {
      BaseUrl = _config.Api,
      _config.Credentials!.Token
    });
  }

  [Fact]
  public void AddHttpCommandDispatcher_ReturnHttpClientWithPreconfiguredTimeout()
  {
    // arrange
    var services = new ServiceCollection();
    services.AddSecTesterConfig(_config);

    // act
    services.AddHttpCommandDispatcher();

    // assert
    using var provider = services.BuildServiceProvider();
    var factory = provider.GetRequiredService<IHttpClientFactory>();
    using var httpClient = factory.CreateClient(nameof(HttpCommandDispatcher));
    httpClient.Should().BeEquivalentTo(new
    {
      Timeout = TimeSpan.FromSeconds(10)
    });
  }

  [Fact]
  public void AddHttpCommandDispatcher_ConfigurationIsNotRegistered_ThrowError()
  {
    // arrange
    var services = new ServiceCollection();

    // act
    services.AddHttpCommandDispatcher();

    // assert
    using var provider = services.BuildServiceProvider();
    Func<ICommandDispatcher> act = () => provider.GetRequiredService<ICommandDispatcher>();
    act.Should().Throw<Exception>();
  }
}
