using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace SecTester.Repeater.Runners;

public interface WsClientFactory
{
  public Task<WebSocket> CreateWsClient(Uri uri, CancellationToken cancellationToken = default);
}

