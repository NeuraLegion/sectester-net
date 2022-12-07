using System;
using System.Net.WebSockets;
using System.Text;

namespace SecTester.Repeater.Runners;

internal sealed class ReceivedMessage
{
  private byte[] Body { get; }
  public int Length => Buffer.ByteLength(Body);
  public WebSocketCloseStatus? StatusCode { get; }

  public ReceivedMessage(byte[] body, WebSocketCloseStatus? statusCode = default)
  {
    Body = body;
    StatusCode = statusCode;
  }

  public override string ToString()
  {
    return Encoding.Default.GetString(Body);
  }
}

