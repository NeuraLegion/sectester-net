using System;
using System.Collections.Generic;
using System.Net.Http;
using SecTester.Core.Bus;

namespace SecTester.Repeater.Commands;

public record HttpRequest<TResult> : Command<TResult>
{
  public HttpContent? Body { get; protected init; }
  public HttpMethod? Method { get; protected init; }
  public IEnumerable<KeyValuePair<string, string>>? Params { get; protected init; }
  public string Url { get; protected init; }

  public HttpRequest(string? url, HttpMethod? method = null, IEnumerable<KeyValuePair<string, string>>? @params = null,
    HttpContent? body = null, bool? expectReply = null, TimeSpan? ttl = null) : base(expectReply, ttl)
  {
    Url = url ?? "/";
    Method = method ?? HttpMethod.Get;
    Params = @params;
    Body = body;
  }
}
