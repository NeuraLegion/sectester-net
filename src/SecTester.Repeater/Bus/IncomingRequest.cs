using System;
using System.Collections.Generic;
using System.Net.Http;
using MessagePack;
using SecTester.Core.Bus;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Bus;

[MessagePackObject(true)]
public record IncomingRequest(Uri Url) : Event, IRequest
{
  public string? Body { get; set; }
  public HttpMethod Method { get; set; } = HttpMethod.Get;
  public Protocol Protocol { get; set; } = Protocol.Http;
  public Uri Url { get; set; } = Url ?? throw new ArgumentNullException(nameof(Url));
  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; } =
    new List<KeyValuePair<string, IEnumerable<string>>>();
}
