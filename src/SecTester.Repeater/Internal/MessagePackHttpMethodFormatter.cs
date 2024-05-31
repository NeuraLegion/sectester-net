using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using MessagePack;
using MessagePack.Formatters;

namespace SecTester.Repeater.Internal;

internal class MessagePackHttpMethodFormatter : IMessagePackFormatter<HttpMethod?>
{
  private static readonly IEnumerable<HttpMethod> BaseMethods = typeof(HttpMethod)
    .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
    .Where(x => x.PropertyType.IsAssignableFrom(typeof(HttpMethod)))
    .Select(x => x.GetValue(null))
    .Cast<HttpMethod>();

  private static readonly IEnumerable<HttpMethod> CustomMethods = new List<HttpMethod>
  {
    new("PATCH"),
    new("COPY"),
    new("LINK"),
    new("UNLINK"),
    new("PURGE"),
    new("LOCK"),
    new("UNLOCK"),
    new("PROPFIND"),
    new("VIEW")
  };

  private static readonly IDictionary<string, HttpMethod> Methods = BaseMethods.Concat(CustomMethods).Distinct()
    .ToDictionary(x => x.Method, x => x, StringComparer.InvariantCultureIgnoreCase);
  public void Serialize(ref MessagePackWriter writer, HttpMethod? value, MessagePackSerializerOptions options)
  {
    if (null == value)
    {
      writer.WriteNil();
    }
    else
    {
      writer.Write(value.Method);
    }
  }

  public HttpMethod? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
  {
    if (reader.NextMessagePackType == MessagePackType.Nil)
    {
      reader.ReadNil();
      return null;
    }

    if (reader.NextMessagePackType != MessagePackType.String)
    {
      throw new MessagePackSerializationException($"Unrecognized code: 0x{reader.NextCode:X2} but expected to be either a string or null.");
    }

    return Deserialize(ref reader);
  }

  private static HttpMethod? Deserialize(ref MessagePackReader reader)
  {
    var token = reader.ReadString();

    if (token is null || !Methods.TryGetValue(token, out var method))
    {
      throw new MessagePackSerializationException(
        $"Unexpected value {token} when parsing the {nameof(HttpMethod)}.");
    }

    return method;
  }
}
