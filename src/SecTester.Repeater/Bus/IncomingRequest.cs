using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
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

  private static readonly Dictionary<string, Protocol> ProtocolEntries = typeof(Protocol)
    .GetFields(BindingFlags.Public | BindingFlags.Static)
    .Select(field => new
    {
      Value = (Protocol)field.GetValue(null),
      StringValue = field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? field.Name
    })
    .ToDictionary(x => x.StringValue, x => x.Value);


  [Key(ProtocolKey)]
  public Protocol Protocol { get; set; } = Protocol.Http;

  [Key(HeadersKey)]
  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; } =
    new List<KeyValuePair<string, IEnumerable<string>>>();

  [Key(BodyKey)]
  public string? Body { get; set; }

  [Key(MethodKey)]
  public HttpMethod Method { get; set; } = HttpMethod.Get;

  [Key(UrlKey)]
  public Uri Url { get; set; } = Url ?? throw new ArgumentNullException(nameof(Url));

  public static IncomingRequest FromDictionary(Dictionary<object, object> dictionary)
  {
    var protocol = dictionary.TryGetValue(ProtocolKey, out var p) && p is string && ProtocolEntries.TryGetValue(p.ToString(), out var e)
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
