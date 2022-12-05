using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using SecTester.Core.Bus;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Bus;

[MessageType(name: "ExecuteScript")]
public record RequestExecutingEvent(Uri Url) : Event, Request
{
  public string? Body { get; init; }
  [JsonPropertyName("correlation_id_regex")]
  public Regex? CorrelationIdRegex { get; init; }
  public HttpMethod Method { get; init; } = HttpMethod.Get;
  public Protocol Protocol { get; init; } = Protocol.Http;
  public Uri Url { get; init; } = Url ?? throw new ArgumentNullException(nameof(Url));

  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; init; } =
    new List<KeyValuePair<string, IEnumerable<string>>>();
}
