using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecTester.Repeater.Internal;

internal class JsonHttpMethodEnumerationStringConverter : JsonConverter<HttpMethod>
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


  public override HttpMethod? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    switch (reader.TokenType)
    {
      case JsonTokenType.String:
        var token = reader.GetString();
        if (token is null || !Methods.TryGetValue(token, out var method))
        {
          throw new JsonException(
            $"Unexpected value {token} when parsing the {nameof(HttpMethod)}.");
        }

        return method;
      case JsonTokenType.Null:
        return null;
      default:
        throw new JsonException(
          $"Unexpected token {reader.TokenType} when parsing the {nameof(HttpMethod)}.");
    }
  }

  public override void Write(Utf8JsonWriter writer, HttpMethod value, JsonSerializerOptions options)
  {
    if (!Methods.TryGetValue(value.Method, out var method))
    {
      throw new JsonException(
        $"Unexpected value {value.Method} when writing the {nameof(HttpMethod)}.");
    }

    writer.WriteStringValue(method.Method);
  }
}
