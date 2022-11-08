using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute.ExceptionExtensions;
using SecTester.Bus.Extensions;

namespace SecTester.Bus.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
  private readonly ServiceCollection _services;
  private readonly Configuration _config;

  public ServiceCollectionExtensionsTests()
  {
    _services = new ServiceCollection();
    _config = new Configuration("app.neuralegion.com",
      new Credentials("0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk"));
  }

  [Fact]
  public void AddSecTesterBus_ReturnHttpCommandDispatcherWithDefaultOptions()
  {
    // arrange
    _services.Add(new ServiceDescriptor(typeof(Configuration), _config));

    // act
    _services.AddSecTesterBus();

    // assert
    using var provider = _services.BuildServiceProvider();
    var result = provider.GetRequiredService<HttpCommandDispatcher>();
    result.Should().BeOfType<HttpCommandDispatcher>();
  }

  [Fact]
  public void AddSecTesterBus_ReturnHttpCommandDispatcherConfig()
  {
    // arrange 
    _services.Add(new ServiceDescriptor(typeof(Configuration), _config));

    // act
    _services.AddSecTesterBus();

    // assert
    using var provider = _services.BuildServiceProvider();
    var result = provider.GetRequiredService<HttpCommandDispatcherConfig>();
    result.Should().BeEquivalentTo(new
    {
      BaseUrl = _config.Api,
      _config.Credentials!.Token
    });
  }

  [Fact]
  public void AddSecTesterBus_ReturnHttpClientWithPreconfiguredTimeout()
  {
    // arrange
    _services.Add(new ServiceDescriptor(typeof(Configuration), _config));

    // act
    _services.AddSecTesterBus();

    // assert
    using var provider = _services.BuildServiceProvider();
    var factory = provider.GetRequiredService<IHttpClientFactory>();
    using var httpClient = factory.CreateClient(nameof(HttpCommandDispatcher));
    httpClient.Should().BeEquivalentTo(new
    {
      Timeout = TimeSpan.FromSeconds(10)
    });
  }

  [Fact]
  public void AddSecTesterBus_ConfigurationIsNotRegistered_ThrowError()
  {
    // act
    _services.AddSecTesterBus();

    // assert
    using var provider = _services.BuildServiceProvider();
    Func<HttpCommandDispatcher> act = () => provider.GetRequiredService<HttpCommandDispatcher>();
    act.Should().Throw<Exception>();
  }
}
