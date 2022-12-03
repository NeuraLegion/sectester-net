using System.Collections.Generic;
using System.Text.Json.Serialization;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Bus;

public record RequestExecutingResult : Response
{
  [JsonPropertyName("status_code")] public int? StatusCode { get; init; }

  public string? Body { get; init; }
  public string? Message { get; init; }

  [JsonPropertyName("error_code")] public string? ErrorCode { get; init; }

  public Protocol Protocol { get; init; } = Protocol.Http;

  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; init; } =
    new List<KeyValuePair<string, IEnumerable<string>>>();
}
