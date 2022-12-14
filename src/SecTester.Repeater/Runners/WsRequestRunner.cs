using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Core.Extensions;
using SecTester.Repeater.Bus;

namespace SecTester.Repeater.Runners;

internal sealed class WsRequestRunner : IRequestRunner
{
  private const WebSocketCloseStatus DefaultStatusCode = WebSocketCloseStatus.NormalClosure;
  private const int MaxBufferSize = 1024 * 4;
  private readonly SemaphoreSlim _lock = new(1, 1);

  private readonly RequestRunnerOptions _options;
  private readonly IWebSocketFactory _webSocketFactory;

  public WsRequestRunner(RequestRunnerOptions options, IWebSocketFactory webSocketFactory)
  {
    _options = options ?? throw new ArgumentNullException(nameof(options));
    _webSocketFactory = webSocketFactory ?? throw new ArgumentNullException(nameof(webSocketFactory));
  }

  public Protocol Protocol => Protocol.Ws;

  public async Task<IResponse> Run(IRequest request)
  {
    using var cts = new CancellationTokenSource(_options.Timeout);

    WebSocket? client = null;

    try
    {
      client = await _webSocketFactory.CreateWebSocket(request.Url, cts.Token).ConfigureAwait(false);

      var result = await SendAndRetrieve(client, request, cts.Token).ConfigureAwait(false);

      return CreateRequestExecutingResult(client, result);
    }
    catch (Exception err)
    {
      return CreateRequestExecutingResult(err);
    }
    finally
    {
      if (client != null)
      {
        await CloseSocket(client, cts.Token).ConfigureAwait(false);
      }
    }
  }

  private async Task<WebSocketResponseBody> SendAndRetrieve(WebSocket client, IRequest request, CancellationToken cancellationToken)
  {
    var message = BuildMessage(request);

    await Send(client, message, cancellationToken).ConfigureAwait(false);

    return await Consume(request, client, cancellationToken).ConfigureAwait(false);
  }

  private async Task Send(WebSocket client, ArraySegment<byte> message, CancellationToken cancellationToken)
  {
    using var _ = await _lock.LockAsync(cancellationToken).ConfigureAwait(false);
    await client.SendAsync(message, WebSocketMessageType.Text, true, cancellationToken).ConfigureAwait(false);
  }

  private static async Task CloseSocket(WebSocket client, CancellationToken cancellationToken)
  {
    try
    {
      if (!client.CloseStatus.HasValue)
      {
        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken).ConfigureAwait(false);
      }

      client.Dispose();
    }
    catch
    {
      // noop
    }
  }

  private static async IAsyncEnumerable<WebSocketResponseBody> ConsumeMessage(WebSocket client,
    [EnumeratorCancellation] CancellationToken cancellationToken)
  {
    using var stream = new MemoryStream();
    var buffer = new ArraySegment<byte>(new byte[MaxBufferSize]);

    while (!client.CloseStatus.HasValue)
    {
      var result = await client.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);

      if (buffer.Array != null)
      {
        await stream.WriteAsync(buffer.Array, buffer.Offset, result.Count, cancellationToken).ConfigureAwait(false);
      }

      if (!result.CloseStatus.HasValue && !result.EndOfMessage)
      {
        continue;
      }

      stream.Seek(0, SeekOrigin.Begin);
      yield return new WebSocketResponseBody(stream.ToArray(), result.CloseStatus, result.CloseStatusDescription);
    }
  }

  private static ValueTask<WebSocketResponseBody> Consume(IRequest request, WebSocket client,
    CancellationToken cancellationToken)
  {
    return ConsumeMessage(client, cancellationToken)
      .FirstAsync(r => request.CorrelationIdRegex is null || request.CorrelationIdRegex.IsMatch(r.ToString()), cancellationToken);
  }

  private static RequestExecutingResult CreateRequestExecutingResult(WebSocket client, WebSocketResponseBody result)
  {
    var closeStatus = result.StatusCode ?? client.CloseStatus ?? DefaultStatusCode;
    var statusDescription = result.StatusDescription ?? client.CloseStatusDescription;

    return new RequestExecutingResult
    {
      Protocol = Protocol.Ws,
      Message = statusDescription,
      StatusCode = (int)closeStatus,
      Body = result.ToString()
    };
  }

  private static RequestExecutingResult CreateRequestExecutingResult(Exception exception)
  {
    var errorCode = GetErrorCode(exception);

    return new RequestExecutingResult
    {
      Protocol = Protocol.Ws,
      Message = exception.Message.TrimEnd(),
      ErrorCode = errorCode
    };
  }

  private static string? GetErrorCode(Exception err)
  {
    // TODO: use native errno codes instead
    return err switch
    {
      WebSocketException exception => Enum.GetName(typeof(WebSocketError), exception.WebSocketErrorCode),
      _ => null
    };
  }

  private static ArraySegment<byte> BuildMessage(IRequest message)
  {
    var buffer = Encoding.Default.GetBytes(message.Body ?? "");
    return new ArraySegment<byte>(buffer);
  }
}
