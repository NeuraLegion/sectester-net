using System.Net.WebSockets;

namespace SecTester.Repeater.Runners;

internal sealed class WebSocketResponseBody : ResponseBody
{
  public WebSocketResponseBody(byte[] body, WebSocketCloseStatus? statusCode = default, string? statusDescription = default) : base(body)
  {
    StatusCode = statusCode;
    StatusDescription = statusDescription;
  }

  public WebSocketCloseStatus? StatusCode { get; }
  public string? StatusDescription { get; }
}


