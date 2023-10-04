using System;
using System.Collections.Generic;
using System.Net.Http;
using SecTester.Core.Bus;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Bus;

public record IncomingRequest(Uri Url) : Event, IRequest
{
  public string? Body { get; init; }
  public HttpMethod Method { get; init; } = HttpMethod.Get;
  public Protocol Protocol { get; init; } = Protocol.Http;
  public Uri Url { get; init; } = Url ?? throw new ArgumentNullException(nameof(Url));

  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; init; } =
    new List<KeyValuePair<string, IEnumerable<string>>>();
}
