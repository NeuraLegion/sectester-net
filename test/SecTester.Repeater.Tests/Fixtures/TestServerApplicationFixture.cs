using SecTester.Repeater.Tests.Mocks;

namespace SecTester.Repeater.Tests.Fixtures;

public class TestServerApplicationFixture<TStartup> : IDisposable, IAsyncDisposable where TStartup : class
{
  private readonly HttpClient _client;
  private readonly TestServerApplicationFactory<TStartup> _factory;

  public TestServerApplicationFixture()
  {
    _factory = new TestServerApplicationFactory<TStartup>();
    _client = _factory.CreateClient(); // This is needed since _factory.Server would otherwise be null
  }

  public Uri Url
  {
    get
    {
      var uri = new UriBuilder(_factory.Server.BaseAddress)
      {
        Scheme = "ws",
        Path = "ws"
      };

      return uri.Uri;
    }
  }

  public void Dispose()
  {
    _factory.Dispose();
    _client.Dispose();
    GC.SuppressFinalize(this);
  }

  public async ValueTask DisposeAsync()
  {
    await _factory.DisposeAsync();
    Dispose();
  }

  public RequestRunner CreateWsRequestRunner(WsClientFactory clientFactory, RequestRunnerOptions? options = default)
  {
    return new WsRequestRunner(options ?? new RequestRunnerOptions(), clientFactory);
  }

  public RequestRunner CreateWsRequestRunner(RequestRunnerOptions? options = default)
  {
    return CreateWsRequestRunner(new MockWsClientFactory(_factory.Server), options);
  }
}




