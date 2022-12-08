namespace SecTester.Repeater.Tests.Mocks;

public class MockWsClientFactory : WsClientFactory, IDisposable
{
  private readonly TestServer _server;

  public MockWsClientFactory(TestServer server)
  {
    _server = server ?? throw new ArgumentNullException(nameof(server));
  }

  public void Dispose()
  {
    _server.Dispose();
    GC.SuppressFinalize(this);
  }

  public Task<WebSocket> CreateWsClient(Uri url, CancellationToken cancellationToken)
  {
    var ws = _server.CreateWebSocketClient();
    return ws.ConnectAsync(url, cancellationToken);
  }
}

