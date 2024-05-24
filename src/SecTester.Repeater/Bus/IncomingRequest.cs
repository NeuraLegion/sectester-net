using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using MessagePack;
using SecTester.Repeater.Bus.Formatters;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Bus;

[MessagePackObject]
public record IncomingRequest(Uri Url): HttpMessage, IRequest
{
  private const string UrlKey = "url";
  private const string MethodKey = "method";

  [Key(MethodKey)]
  [MessagePackFormatter(typeof(MessagePackHttpMethodFormatter))]
  public HttpMethod Method { get; set; } = HttpMethod.Get;

  [Key(UrlKey)]
  public Uri Url { get; set; } = Url ?? throw new ArgumentNullException(nameof(Url));

  public static IncomingRequest FromDictionary(Dictionary<object, object> dictionary)
  {
    var protocol = dictionary.TryGetValue(ProtocolKey, out var p) && p is string && Enum.TryParse<Protocol>(p.ToString(), out var e)
      ? e
      : Protocol.Http;

    var uri = dictionary.TryGetValue(UrlKey, out var u) && u is string
      ? new Uri(u.ToString())
      : throw new InvalidDataException(FormatPropertyError(UrlKey));

    var method = dictionary.TryGetValue(MethodKey, out var m) && m is string
      ? new HttpMethod(m.ToString())
      : HttpMethod.Get;

    var body = dictionary.TryGetValue(BodyKey, out var b) && b is string ? b.ToString() : null;

    var headers = dictionary.TryGetValue(HeadersKey, out var h) && h is Dictionary<object, object> value
      ? MapHeaders(value)
      : new List<KeyValuePair<string, IEnumerable<string>>>();

    return new IncomingRequest(uri)
    {
      Protocol = protocol,
      Body = body,
      Method = method,
      Headers = headers
    };
  }

  private static IEnumerable<KeyValuePair<string, IEnumerable<string>>> MapHeaders(Dictionary<object, object> headers)
  {
    var result = new List<KeyValuePair<string, IEnumerable<string>>>(headers?.Count ?? 0);

    foreach (var kvp in headers)
    {
      var key = kvp.Key.ToString();

      switch (kvp.Value)
      {
        case null:
          result.Add(new KeyValuePair<string, IEnumerable<string>>(key, new List<string>()));
          continue;
        case string:
          result.Add(new KeyValuePair<string, IEnumerable<string>>(key, new List<string>
            { kvp.Value.ToString() }));
          continue;
        case object[] objects:
          result.Add(new KeyValuePair<string, IEnumerable<string>>(key,
            objects.OfType<string>().Select(value => value.ToString()).ToList()));
          continue;
      }
    }

    return result;
  }

  private static string FormatPropertyError(string propName) => $"{propName} is either null or has an invalid data type";
}
