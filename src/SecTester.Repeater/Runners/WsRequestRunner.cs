using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Repeater.Bus;

namespace SecTester.Repeater.Runners;

internal sealed class WsRequestRunner : RequestRunner
{
  private const int DefaultStatusCode = 1000;
  private const int MaxBufferSize = 1024 * 4;

  private readonly RequestRunnerOptions _options;

  public WsRequestRunner(RequestRunnerOptions options)
  {
    _options = options ?? throw new ArgumentNullException(nameof(options));
  }

  public Protocol Protocol => Protocol.Ws;

  public async Task<Response> Run(Request request)
  {
    using var cts = new CancellationTokenSource(_options.Timeout);
    using var client = CreateWebSocketClient();

    try
    {
      await client.ConnectAsync(request.Url, cts.Token).ConfigureAwait(false);

      var message = BuildMessage(request);

      await client.SendAsync(message, WebSocketMessageType.Text, true, cts.Token).ConfigureAwait(false);

      return await Consume(request, client, cts.Token).ConfigureAwait(false);
    }
    catch (Exception err)
    {
      return CreateRequestExecutingResult(err);
    }
    finally
    {
      await CloseSocket(client, cts.Token).ConfigureAwait(false);
    }
  }

  private static async Task CloseSocket(WebSocket client, CancellationToken cancellationToken)
  {
    try
    {
      if (!client.CloseStatus.HasValue)
      {
        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken).ConfigureAwait(false);
      }
    }
    catch
    {
      // noop
    }
  }

  private async IAsyncEnumerable<ReceivedMessage> ConsumeMessage(WebSocket client,
    [EnumeratorCancellation] CancellationToken cancellationToken)
  {
    using var stream = new MemoryStream();
    var buffer = new ArraySegment<byte>(new byte[MaxBufferSize]);

    while (!client.CloseStatus.HasValue)
    {
      var result = await client.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
      await stream.WriteAsync(buffer.Array, buffer.Offset, result.Count, cancellationToken).ConfigureAwait(false);

      if (result.CloseStatus.HasValue || result.EndOfMessage)
      {
        yield return new ReceivedMessage(stream.ToArray(), result.CloseStatus);
      }
    }
  }

  private async Task<RequestExecutingResult> Consume(Request request, WebSocket client,
    CancellationToken cancellationToken)
  {
    var result = await ConsumeMessage(client, cancellationToken)
      .FirstAsync(r => request.CorrelationIdRegex is null || request.CorrelationIdRegex.IsMatch(r.ToString()), cancellationToken)
      .ConfigureAwait(false);

    return CreateRequestExecutingResult(client, result);
  }

  private static RequestExecutingResult CreateRequestExecutingResult(WebSocket client, ReceivedMessage result)
  {
    return new RequestExecutingResult
    {
      Protocol = Protocol.Ws,
      StatusCode = (result.StatusCode ?? client.CloseStatus) as int? ?? DefaultStatusCode,
      Body = result.ToString()
    };
  }

  private static RequestExecutingResult CreateRequestExecutingResult(Exception exception)
  {
    var errorCode = GetErrorCode(exception);

    return new RequestExecutingResult
    {
      Protocol = Protocol.Ws,
      Message = exception.Message,
      ErrorCode = errorCode
    };
  }

  private static string? GetErrorCode(Exception err)
  {
    // TODO: use native errno codes instead
    return err switch
    {
      SocketException exception => Enum.GetName(typeof(SocketError), exception.SocketErrorCode),
      WebSocketException exception => Enum.GetName(typeof(WebSocketError), exception.WebSocketErrorCode),
      _ => null
    };
  }

  private static ArraySegment<byte> BuildMessage(Request message)
  {
    var buffer = Encoding.Default.GetBytes(message.Body ?? "");
    return new ArraySegment<byte>(buffer);
  }

  private ClientWebSocket CreateWebSocketClient()
  {
    var proxy = _options.ProxyUrl is not null ? new WebProxy(_options.ProxyUrl) : null;
    // TODO: disable certs validation. For details see https://github.com/dotnet/runtime/issues/18696
    return new ClientWebSocket
    {
      Options =
      {
        Proxy = proxy, KeepAliveInterval = _options.Timeout
      }
    };
  }
}





