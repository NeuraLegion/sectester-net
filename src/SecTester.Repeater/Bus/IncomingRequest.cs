using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MessagePack;
using SecTester.Repeater.Internal;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Bus;

[MessagePackObject]
public record IncomingRequest(Uri Url) : IRequest
{
  private const string UrlKey = "url";
  private const string MethodKey = "method";
  private const string HeadersKey = "headers";
  private const string BodyKey = "body";
  private const string ProtocolKey = "protocol";

  [Key(ProtocolKey)] public Protocol Protocol { get; set; } = Protocol.Http;

  [Key(HeadersKey)] public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; } = new Dictionary<string, IEnumerable<string>>();

  [Key(BodyKey)] public string? Body { get; set; }

  [Key(MethodKey)] public HttpMethod Method { get; set; } = HttpMethod.Get;

  [Key(UrlKey)] public Uri Url { get; set; } = Url ?? throw new ArgumentNullException(nameof(Url));

  public static IncomingRequest FromDictionary(Dictionary<object, object> dictionary)
  {
    var protocol = GetProtocolFromDictionary(dictionary);
    var headers = GetHeadersFromDictionary(dictionary);
    var body = GetBodyFromDictionary(dictionary);
    var method = GetMethodFromDictionary(dictionary);
    var url = GetUrlFromDictionary(dictionary);

    return new IncomingRequest(url!)
    {
      Protocol = protocol,
      Headers = headers,
      Body = body,
      Method = method
    };
  }

  private static Protocol GetProtocolFromDictionary(Dictionary<object, object> dictionary) =>
    dictionary.TryGetValue(ProtocolKey, out var protocolObj) && protocolObj is string protocolStr
      ? (Protocol)Enum.Parse(typeof(Protocol), protocolStr, true)
      : Protocol.Http;

  private static IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetHeadersFromDictionary(Dictionary<object, object> dictionary) =>
    dictionary.TryGetValue(HeadersKey, out var headersObj) && headersObj is Dictionary<object, object> headersDict
      ? ConvertToHeaders(headersDict)
      : new Dictionary<string, IEnumerable<string>>();

  private static string? GetBodyFromDictionary(Dictionary<object, object> dictionary) =>
    dictionary.TryGetValue(BodyKey, out var bodyObj) ? bodyObj?.ToString() : null;

  private static HttpMethod GetMethodFromDictionary(Dictionary<object, object> dictionary) =>
    dictionary.TryGetValue(MethodKey, out var methodObj) && methodObj is string methodStr
      ? HttpMethods.Items.TryGetValue(methodStr, out var m) && m is not null
        ? m
        : HttpMethod.Get
      : HttpMethod.Get;

  private static Uri? GetUrlFromDictionary(Dictionary<object, object> dictionary) =>
    dictionary.TryGetValue(UrlKey, out var urlObj) && urlObj is string urlStr
      ? new Uri(urlStr)
      : null;

  private static IEnumerable<KeyValuePair<string, IEnumerable<string>>> ConvertToHeaders(Dictionary<object, object> headers) =>
    headers.ToDictionary(
      kvp => kvp.Key.ToString()!,
      kvp => kvp.Value switch
      {
        IEnumerable<object> list => list.Select(v => v.ToString()!),
        string str => new[] { str },
        _ => Enumerable.Empty<string>()
      }
    );
}
