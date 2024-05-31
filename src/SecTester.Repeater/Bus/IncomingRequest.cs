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
    .ToDictionary(x => MessagePackNamingPolicy.SnakeCase.ConvertName(x.StringValue), x => x.Value);

  [Key(ProtocolKey)]
  public Protocol Protocol { get; set; } = Protocol.Http;

  private IEnumerable<KeyValuePair<string, IEnumerable<string>>> _headers = Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();

  [Key(HeadersKey)]
  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers
  {
    get => _headers;
    // ADHOC: convert from a kind of assignable type to formatter resolvable type
    set => _headers = value.AsEnumerable();
  }

  [Key(BodyKey)]
  public string? Body { get; set; }

  [Key(MethodKey)]
  public HttpMethod Method { get; set; } = HttpMethod.Get;

  [Key(UrlKey)]
  public Uri Url { get; set; } = Url ?? throw new ArgumentNullException(nameof(Url));

  public static IncomingRequest FromDictionary(Dictionary<object, object> dictionary)
  {
    var protocol = !dictionary.ContainsKey(ProtocolKey) || (dictionary.TryGetValue(ProtocolKey, out var p1) && p1 is null)
      ? Protocol.Http
      : dictionary.TryGetValue(ProtocolKey, out var p2) && p2 is string && ProtocolEntries.TryGetValue(p2.ToString(), out var e)
        ? e
        : throw new InvalidDataException(FormatPropertyError(ProtocolKey));

    var uri = dictionary.TryGetValue(UrlKey, out var u) && u is string
      ? new Uri(u.ToString())
      : throw new InvalidDataException(FormatPropertyError(UrlKey));

    var method = dictionary.TryGetValue(MethodKey, out var m) && m is string
      ? new HttpMethod(m.ToString())
      : HttpMethod.Get;

    var body = dictionary.TryGetValue(BodyKey, out var b) && b is string ? b.ToString() : null;

    var headers = dictionary.TryGetValue(HeadersKey, out var h) && h is Dictionary<object, object> value
      ? MapHeaders(value)
      : Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();

    return new IncomingRequest(uri)
    {
      Protocol = protocol,
      Body = body,
      Method = method,
      Headers = headers
    };
  }

  private static IEnumerable<KeyValuePair<string, IEnumerable<string>>> MapHeaders(Dictionary<object, object> headers) =>
    headers.Select(kvp => kvp.Value switch
    {
      IEnumerable<object> strings => new KeyValuePair<string, IEnumerable<string>>(kvp.Key.ToString(), strings.Select(x => x.ToString())),
      null => new KeyValuePair<string, IEnumerable<string>>(kvp.Key.ToString(), Enumerable.Empty<string>()),
      _ => new KeyValuePair<string, IEnumerable<string>>(kvp.Key.ToString(), new[] { kvp.Value.ToString() })
    });

  private static string FormatPropertyError(string propName) => $"{propName} is either null or has an invalid data type or value";
}
