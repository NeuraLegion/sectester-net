using System.Collections.Generic;
using MessagePack;
using SecTester.Repeater.Bus.Formatters;

namespace SecTester.Repeater.Bus;

public record HttpMessage
{
  public const string HeadersKey = "headers";
  public const string BodyKey = "body";
  public const string ProtocolKey = "protocol";

  [Key(ProtocolKey)]
  public Protocol Protocol { get; set; } = Protocol.Http;

  [Key(HeadersKey)]
  [MessagePackFormatter(typeof(MessagePackHttpHeadersFormatter))]
  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; } =
    new List<KeyValuePair<string, IEnumerable<string>>>();

  [Key(BodyKey)]
  public string? Body { get; set; }
}
