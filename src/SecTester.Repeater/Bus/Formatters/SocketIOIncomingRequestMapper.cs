using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace SecTester.Repeater.Bus.Formatters;

public class SocketIOIncomingRequestMapper
{
  private const string Protocol = "protocol";
  private const string Url = "url";
  private const string Method = "method";
  private const string Body = "body";
  private const string Headers = "headers";

  public static IncomingRequest ToRequest(Dictionary<object, object> dictionary)
  {
    var protocol = dictionary.TryGetValue(Protocol, out var p) && p is string && Enum.TryParse<Protocol>(p.ToString(), out var e)
      ? e
      : SecTester.Repeater.Protocol.Http;

    var uri = dictionary.TryGetValue(Url, out var u) && u is string
      ? new Uri(u.ToString())
      : throw new InvalidDataException(FormatPropertyError(Url));

    var method = dictionary.TryGetValue(Method, out var m) && m is string
      ? new HttpMethod(m.ToString())
      : HttpMethod.Get;

    var body = dictionary.TryGetValue(Body, out var b) && b is string ? b.ToString() : null;

    dictionary.TryGetValue(Headers, out var headers);

    return new IncomingRequest(uri)
    {
      Protocol = protocol,
      Body = body,
      Method = method,
      Headers = MapHeaders(headers as Dictionary<object, object>)
    };
  }

  private static IEnumerable<KeyValuePair<string, IEnumerable<string>>> MapHeaders(Dictionary<object, object>? headers)
  {
    var result = new List<KeyValuePair<string, IEnumerable<string>>>(headers?.Count ?? 0);

    if (null == headers)
    {
      return result;
    }

    foreach (var kvp in headers)
    {
      var key = kvp.Key.ToString();

      if (null == kvp.Value)
      {
        result.Add(new KeyValuePair<string, IEnumerable<string>>(key, new List<string>()));
        continue;
      }

      if (kvp.Value is string)
      {
        result.Add(new KeyValuePair<string, IEnumerable<string>>(key, new List<string>
          { kvp.Value.ToString() }));
        continue;
      }

      if (kvp.Value is not object[] objects)
      {
        continue;
      }

      var values = objects.OfType<string>().Select(value => value.ToString()).ToList();

      result.Add(new KeyValuePair<string, IEnumerable<string>>(key, values));
    }

    return result;
  }

  private static string FormatPropertyError(string propName) => $"{propName} is either null or has an invalid data type";
}
