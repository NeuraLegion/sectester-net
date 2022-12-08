using System.Globalization;

namespace SecTester.Repeater.Tests.Fixtures;

// This is from https://github.com/aspnet/AspNetCore.Docs/blob/master/aspnetcore/fundamentals/websockets/samples/2.x/WebSocketsSample/Startup.cs
public sealed class Startup
{
  public void Configure(IApplicationBuilder app)
  {
    app.UseWebSockets();
    app.Use(async (context, next) =>
    {
      switch (context.Request.Path)
      {
        case "/ws":
          if (context.WebSockets.IsWebSocketRequest)
          {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await HandleRequest(webSocket);
          }
          else
          {
            context.Response.StatusCode = 400;
          }

          break;
        default:
          await next();
          break;
      }
    });
  }

  private async Task HandleRequest(WebSocket webSocket)
  {
    while (true)
    {
      var (result, message) = await ReadRequest(webSocket);
      if (result.CloseStatus.HasValue)
      {
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        return;
      }

      await HandleRequest(webSocket, message!);
    }
  }

  private Task HandleRequest(WebSocket webSocket, byte[] request)
  {
    var msg = Encoding.Default.GetString(request ?? Array.Empty<byte>()).Trim().ToLower(CultureInfo.InvariantCulture);

    return msg switch
    {
      "ping" => SendEcho(webSocket, "pong"),
      "range" => Task.WhenAll(Enumerable.Range(0, 5).Select(idx => SendEcho(webSocket, $"range:{idx}"))),
      "chunked" => SendChunkedEcho(webSocket, "ping pong"),
      not null when msg.StartsWith("fast") => SendEcho(webSocket, msg),
      not null when msg.StartsWith("echo") => SendEcho(webSocket, msg, true),
      "close" => webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "normal closure", CancellationToken.None),
      "close-output" => webSocket.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData, "invalid payload", CancellationToken.None),
      _ => SendEcho(webSocket, msg)
    };
  }


  private static async Task<(WebSocketReceiveResult result, byte[]? message)> ReadRequest(WebSocket webSocket)
  {
    var buffer = new ArraySegment<byte>(new byte[8192]);
    using var stream = new MemoryStream();
    WebSocketReceiveResult? result;
    do
    {
      result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
      if (result.CloseStatus.HasValue)
      {
        return (result, null);
      }

      if (buffer.Array != null)
      {
        stream.Write(buffer.Array, buffer.Offset, result.Count);
      }
    } while (!result.EndOfMessage);

    stream.Seek(0, SeekOrigin.Begin);

    return (result, stream.ToArray());
  }

  private static Encoding GetEncoding()
  {
    return Encoding.UTF8;
  }

  private async Task SendEcho(WebSocket webSocket, string msg, bool slowdown = false)
  {
    if (slowdown)
    {
      await Task.Delay(100);
    }

    await Send(webSocket, msg, slowdown: slowdown);
  }

  private async Task SendChunkedEcho(WebSocket webSocket, string msg, bool slowdown = false)
  {
    var idx = (int)Math.Floor((double)msg.Length / 2);

    await Send(webSocket, msg[..idx], endOfMessage: false, slowdown: slowdown);
    await Send(webSocket, msg[idx..], slowdown: slowdown);
  }

  private async Task Send(WebSocket webSocket, string msg, bool endOfMessage = true, bool slowdown = false)
  {
    if (slowdown)
    {
      await Task.Delay(100);
    }

    var encoding = GetEncoding();
    var bytes = encoding.GetBytes(msg);
    var segment = new ArraySegment<byte>(bytes);

    await webSocket.SendAsync(
      segment,
      WebSocketMessageType.Text,
      endOfMessage,
      CancellationToken.None);
  }
}
