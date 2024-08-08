namespace SecTester.Repeater.Tests.Extensions;

public class ServiceCollectionExtensionsTests : IDisposable
{
  private readonly ServiceCollection _sut = Substitute.ForPartsOf<ServiceCollection>();

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
}
