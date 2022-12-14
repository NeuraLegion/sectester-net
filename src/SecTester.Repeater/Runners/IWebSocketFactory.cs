using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace SecTester.Repeater.Runners;

public interface IWebSocketFactory
{
  public Task<WebSocket> CreateWebSocket(Uri uri, CancellationToken cancellationToken = default);
}

