using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Repeater.Bus;

namespace SecTester.Repeater.Runners;

internal sealed class WsRequestRunner : RequestRunner
{
  private const WebSocketCloseStatus DefaultStatusCode = WebSocketCloseStatus.NormalClosure;
  private const int MaxBufferSize = 1024 * 4;

  private readonly RequestRunnerOptions _options;
  private readonly WsClientFactory _wsClientFactory;

  public WsRequestRunner(RequestRunnerOptions options, WsClientFactory wsClientFactory)
  {
    _options = options ?? throw new ArgumentNullException(nameof(options));
    _wsClientFactory = wsClientFactory ?? throw new ArgumentNullException(nameof(wsClientFactory));
  }

  public Protocol Protocol => Protocol.Ws;

  public async Task<Response> Run(Request request)
  {
    using var cts = new CancellationTokenSource(_options.Timeout);
    // TODO: handle possible WebSocketException
    using var client = await _wsClientFactory.CreateWsClient(request.Url, cts.Token).ConfigureAwait(false);

    try
    {
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

  private async IAsyncEnumerable<WsResponseBody> ConsumeMessage(WebSocket client,
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
      yield return new WsResponseBody(stream.ToArray(), result.CloseStatus, result.CloseStatusDescription);
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

  private static RequestExecutingResult CreateRequestExecutingResult(WebSocket client, WsResponseBody result)
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

  private static ArraySegment<byte> BuildMessage(Request message)
  {
    var buffer = Encoding.Default.GetBytes(message.Body ?? "");
    return new ArraySegment<byte>(buffer);
  }
}
