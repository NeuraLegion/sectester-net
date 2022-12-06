using System;
using System.Text;

namespace SecTester.Repeater.Runners;

internal sealed class TruncatedBody
{
  public TruncatedBody(byte[] body, string? charSet = default)
  {
    Body = body;
    Encoding = string.IsNullOrEmpty(charSet) ? Encoding.Default : Encoding.GetEncoding(charSet);
  }

  private Encoding Encoding { get; }
  private byte[] Body { get; }
  public int Length => Buffer.ByteLength(Body);

  public override string ToString()
  {
    return Encoding.GetString(Body);
  }
}
