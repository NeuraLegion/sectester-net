using System.Collections.Generic;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Bus;

public record OutgoingResponse : IResponse
{
  public int? StatusCode { get; init; }

  public string? Body { get; init; }
  public string? Message { get; init; }

  public string? ErrorCode { get; init; }

  public Protocol Protocol { get; init; } = Protocol.Http;

  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; init; } =
    new List<KeyValuePair<string, IEnumerable<string>>>();
}
