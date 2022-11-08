using System.Collections.Generic;
using System.Net.Http;
using SecTester.Core.Bus;

namespace SecTester.Bus.Commands;

public record HttpRequest<TResult> : Command<TResult>
{
  public readonly string? Body;
  public readonly HttpMethod? Method;
  public readonly Dictionary<string, string>? Params;
  public readonly string Url;

  public HttpRequest(string? url, HttpMethod? method = null, Dictionary<string, string>? @params = null,
    string? body = null, bool? expectReply = null, int? ttl = null) : base(expectReply, ttl)
  {
    Url = url ?? "/";
    Method = method ?? HttpMethod.Get;
    Params = @params;
    Body = body;
  }
}
