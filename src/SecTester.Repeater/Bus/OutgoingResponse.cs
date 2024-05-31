using System.Collections.Generic;
using System.Linq;
using MessagePack;
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

  private IEnumerable<KeyValuePair<string, IEnumerable<string>>> _headers = Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();

  [Key("headers")]
  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers
  {
    get => _headers;
    // ADHOC: convert from a kind of assignable type to formatter resolvable type
    set => _headers = value.AsEnumerable();
  }
}
