using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace SecTester.Repeater.Runners;

public class DefaultWebSocketFactory : IWebSocketFactory
{
  private readonly RequestRunnerOptions _options;

  public DefaultWebSocketFactory(RequestRunnerOptions options)
  {
    _options = options ?? throw new ArgumentNullException(nameof(options));
  }

  public async Task<WebSocket> CreateWebSocket(Uri uri, CancellationToken cancellationToken = default)
  {
    var proxy = _options.ProxyUrl is not null ? new WebProxy(_options.ProxyUrl) : null;
    // TODO: disable certs validation. For details see https://github.com/dotnet/runtime/issues/18696
    var client = new ClientWebSocket
    {
      Options =
      {
        Proxy = proxy, KeepAliveInterval = _options.Timeout
      }
    };

    await client.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);

    return client;
  }
}

