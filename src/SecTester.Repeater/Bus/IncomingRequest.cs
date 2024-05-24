using System;
using System.Collections.Generic;
using System.Net.Http;
using MessagePack;
using SecTester.Core.Bus;
using SecTester.Repeater.Bus.Formatters;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Bus;

[MessagePackObject]
public record IncomingRequest(Uri Url) : Event, IRequest
{
  [Key("body")]
  public string? Body { get; set; }

  [Key("method")]
  [MessagePackFormatter(typeof(MessagePackHttpMethodFormatter))]
  public HttpMethod Method { get; set; } = HttpMethod.Get;

  [Key("protocol")]
  public Protocol Protocol { get; set; } = Protocol.Http;

  [Key("url")]
  public Uri Url { get; set; } = Url ?? throw new ArgumentNullException(nameof(Url));

  [Key("headers")]
  [MessagePackFormatter(typeof(MessagePackHttpHeadersFormatter))]
  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; } =
    new List<KeyValuePair<string, IEnumerable<string>>>();
}
