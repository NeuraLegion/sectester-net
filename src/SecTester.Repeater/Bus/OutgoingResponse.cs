using System.Collections.Generic;
using MessagePack;
using SecTester.Repeater.Internal;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Bus;

[MessagePackObject]
public record OutgoingResponse : IResponse
{
  [Key("protocol")]
  public Protocol Protocol { get; set; } = Protocol.Http;

  [Key("statusCode")]
  public int? StatusCode { get; set; }

  [Key("body")]
  public string? Body { get; set; }

  [Key("message")]
  public string? Message { get; set; }

  [Key("errorCode")]
  public string? ErrorCode { get; set; }

  [Key("headers")]
  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; } =
    new List<KeyValuePair<string, IEnumerable<string>>>();

}
