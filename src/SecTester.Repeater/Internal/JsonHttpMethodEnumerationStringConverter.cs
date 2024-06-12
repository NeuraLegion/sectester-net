using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecTester.Repeater.Internal;

internal class JsonHttpMethodEnumerationStringConverter : JsonConverter<HttpMethod>
{
  public override HttpMethod? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    switch (reader.TokenType)
    {
      case JsonTokenType.String:
        var token = reader.GetString();
        if (token is null || !HttpMethods.Items.TryGetValue(token, out var method))
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
    if (!HttpMethods.Items.TryGetValue(value.Method, out var method))
    {
      throw new JsonException(
        $"Unexpected value {value.Method} when writing the {nameof(HttpMethod)}.");
    }

    writer.WriteStringValue(method.Method);
  }
}
