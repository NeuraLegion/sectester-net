using System.Collections.Generic;
using MessagePack;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Bus;

[MessagePackObject(true)]
public record OutgoingResponse : IResponse
{
  public int? StatusCode { get; set; }
  public string? Body { get; set; }
  public string? Message { get; set; }
  public string? ErrorCode { get; set; }
  public Protocol Protocol { get; set; } = Protocol.Http;
  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; } =
    new List<KeyValuePair<string, IEnumerable<string>>>();
}
