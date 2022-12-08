namespace SecTester.Repeater.Tests.Mocks;

public class MockWebSocketFactory : WebSocketFactory, IDisposable
{
  private readonly TestServer _server;

  public MockWebSocketFactory(TestServer server)
  {
    _server = server ?? throw new ArgumentNullException(nameof(server));
  }

  public void Dispose()
  {
    _server.Dispose();
    GC.SuppressFinalize(this);
  }

  public Task<WebSocket> CreateWebSocket(Uri url, CancellationToken cancellationToken)
  {
    var ws = _server.CreateWebSocketClient();
    return ws.ConnectAsync(url, cancellationToken);
  }
}

