using System.Net.WebSockets;
using Microsoft.AspNetCore.Builder;

namespace SecTester.Repeater.Tests.Fixtures;

// This is from https://github.com/aspnet/AspNetCore.Docs/blob/master/aspnetcore/fundamentals/websockets/samples/2.x/WebSocketsSample/Startup.cs
public sealed class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    // noop
  }

  public void Configure(IApplicationBuilder app)
  {
    app.UseWebSockets();
    app.Use(async (context, next) =>
    {
      if (context.Request.Path == "/ws")
      {
        if (context.WebSockets.IsWebSocketRequest)
        {
          var webSocket = await context.WebSockets.AcceptWebSocketAsync();
          await HandleRequest(webSocket);
        }
        else
        {
          context.Response.StatusCode = 400;
        }
      }
      else
      {
        await next();
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
    var msg = (Encoding.Default.GetString(request) ?? string.Empty).Trim().ToLower();

    return msg switch
    {
      "ping" => SendEcho(webSocket, "pong"),
      not null when msg.StartsWith("echo_fast") => SendEcho(webSocket, msg),
      not null when msg.StartsWith("echo") => SendEcho(webSocket, msg, true),
      "close-me" => webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "normal closure", CancellationToken.None),
      _ => SendEcho(webSocket, msg),
    };
  }


  private async Task<(WebSocketReceiveResult result, byte[]? message)> ReadRequest(WebSocket webSocket)
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

  private Encoding GetEncoding()
  {
    return Encoding.UTF8;
  }

  private async Task SendEcho(WebSocket webSocket, string msg, bool slowdown = false)
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
      true,
      CancellationToken.None);
  }
}



