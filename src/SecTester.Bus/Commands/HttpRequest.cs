using System.Collections.Generic;
using System.Net.Http;
using SecTester.Core.Bus;

namespace SecTester.Bus.Commands;

public record HttpRequest<TResult> : Command<TResult>
{
  public string? Body { get; protected init; }
  public HttpMethod? Method { get; protected init; }
  public Dictionary<string, string>? Params { get; protected init; }
  public string Url { get; protected init; }

  public HttpRequest(string? url, HttpMethod? method = null, Dictionary<string, string>? @params = null,
    string? body = null, bool? expectReply = null, int? ttl = null) : base(expectReply, ttl)
  {
    Url = url ?? "/";
    Method = method ?? HttpMethod.Get;
    Params = @params;
    Body = body;
  }
}
