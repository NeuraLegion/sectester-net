namespace SecTester.Repeater.Tests.Extensions;

public class ServiceCollectionExtensionsTests : IDisposable
{
  private readonly ServiceCollection _sut = Substitute.ForPartsOf<ServiceCollection>();
  private readonly Configuration _config = new("app.brightsec.com",
    new Credentials("0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk"));

  public void Dispose()
  {
    _sut.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public void AddSecTesterRepeater_RegistersRepeaterFactory()
  {
    // act
    _sut.AddSecTesterRepeater();

    // assert
    _sut.Received().AddScoped<IRepeaterFactory, DefaultRepeaterFactory>();
  }

  [Fact]
  public void AddSecTesterRepeater_RegistersRepeaters()
  {
    // act
    _sut.AddSecTesterRepeater();

    // assert
    _sut.Received().AddScoped<IRepeaters, DefaultRepeaters>();
  }

  [Fact]
  public void AddSecTesterRepeater_RegistersTimerProvider()
  {
    // act
    _sut.AddSecTesterRepeater();

    // assert
    _sut.Received().AddScoped<ITimerProvider, SystemTimerProvider>();
  }

  [Fact]
  public void AddSecTesterRepeater_WithTimeout_RegistersHttpClient()
  {
    // arrange
    var timeout = TimeSpan.FromSeconds(30);

    // act
    _sut.AddSecTesterRepeater();

    // assert
    using var provider = _sut.BuildServiceProvider();
    var result = provider.GetRequiredService<IHttpClientFactory>();
    using var client = result.CreateClient(nameof(HttpRequestRunner));

    client.Timeout.Should().Be(timeout);
  }

  [Fact]
  public void AddSecTesterRepeater_RegistersRequestRunnerRegistry()
  {
    // act
    _sut.AddSecTesterRepeater();

    // assert
    using var provider = _sut.BuildServiceProvider();
    var result = provider.GetRequiredService<RequestRunnerResolver>();

    result(Protocol.Http).Should().BeOfType(typeof(HttpRequestRunner));
  }

  [Fact]
  public void AddSecTesterRepeater_WithKeepAlive_RegistersHttpClient()
  {
    // arrange
    var timeout = TimeSpan.FromSeconds(1);


    // act
    _sut.AddSecTesterRepeater(new RequestRunnerOptions
    {
      Timeout = timeout,
      ReuseConnection = true
    });

    // assert
    using var provider = _sut.BuildServiceProvider();
    var result = provider.GetRequiredService<IHttpClientFactory>();
    using var client = result.CreateClient(nameof(HttpRequestRunner));

    client.DefaultRequestHeaders.Should().BeEquivalentTo(new[]
    {
      new KeyValuePair<string, IEnumerable<string>>("Connection", new[]
      {
        "keep-alive"
      }),
      new KeyValuePair<string, IEnumerable<string>>("Keep-Alive", new[]
      {
        $"{timeout}"
      })
    });
  }

  [Fact]
  public void AddSecTesterRepeater_WithExtraHeaders_RegistersHttpClient()
  {
    // arrange
    var headers = new List<KeyValuePair<string, IEnumerable<string>>>
    {
      new("x-header", new []{"x-value"})
    };

    // act
    _sut.AddSecTesterRepeater(new RequestRunnerOptions
    {
      Headers = headers,
    });

    // assert
    using var provider = _sut.BuildServiceProvider();
    var result = provider.GetRequiredService<IHttpClientFactory>();
    using var client = result.CreateClient(nameof(HttpRequestRunner));

    client.DefaultRequestHeaders.Should().BeEquivalentTo(headers);
  }

  [Fact]
  public void AddSecTesterRepeater_ReturnHttpCommandDispatcherWithDefaultOptions()
  {
    // arrange
    _sut.Add(new ServiceDescriptor(typeof(Configuration), _config));

    // act
    _sut.AddSecTesterRepeater();

    // assert
    using var provider = _sut.BuildServiceProvider();
    var result = provider.GetRequiredService<ICommandDispatcher>();
    result.Should().BeOfType<HttpCommandDispatcher>();
  }

  [Fact]
  public void AddSecTesterRepeater_ReturnHttpCommandDispatcherConfig()
  {
    // arrange
    _sut.Add(new ServiceDescriptor(typeof(Configuration), _config));

    // act
    _sut.AddSecTesterRepeater();

    // assert
    using var provider = _sut.BuildServiceProvider();
    var result = provider.GetRequiredService<HttpCommandDispatcherConfig>();
    result.Should().BeEquivalentTo(new
    {
      BaseUrl = _config.Api,
      _config.Credentials!.Token
    });
  }

  [Fact]
  public void AddSecTesterRepeater_ReturnHttpClientWithPreconfiguredTimeout()
  {
    // arrange
    _sut.Add(new ServiceDescriptor(typeof(Configuration), _config));

    // act
    _sut.AddSecTesterRepeater();

    // assert
    using var provider = _sut.BuildServiceProvider();
    var factory = provider.GetRequiredService<IHttpClientFactory>();
    using var httpClient = factory.CreateClient(nameof(HttpCommandDispatcher));
    httpClient.Should().BeEquivalentTo(new
    {
      Timeout = TimeSpan.FromSeconds(10)
    });
  }

  [Fact]
  public void AddSecTesterRepeater_ConfigurationIsNotRegistered_ThrowError()
  {
    // act
    _sut.AddSecTesterRepeater();

    // assert
    using var provider = _sut.BuildServiceProvider();
    Func<ICommandDispatcher> act = () => provider.GetRequiredService<ICommandDispatcher>();
    act.Should().Throw<Exception>();
  }
}
