using System;
using System.Net.WebSockets;
using System.Text;

namespace SecTester.Repeater.Runners;

internal sealed class ReceivedMessage
{
  private byte[] Body { get; }
  public WebSocketCloseStatus? StatusCode { get; }
  public string? StatusDescription { get; }

  public ReceivedMessage(byte[] body, WebSocketCloseStatus? statusCode = default, string? statusDescription = default)
  {
    Body = body;
    StatusCode = statusCode;
    StatusDescription = statusDescription;
  }

  public override string ToString()
  {
    return Encoding.Default.GetString(Body);
  }
}

