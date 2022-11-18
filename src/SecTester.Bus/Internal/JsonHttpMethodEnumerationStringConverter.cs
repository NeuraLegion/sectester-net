using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecTester.Bus.Internal;

internal class JsonHttpMethodEnumerationStringConverter : JsonConverter<HttpMethod>
{
  private static readonly HttpMethod _patch = new("PATCH");
  private static readonly HttpMethod _copy = new("COPY");
  private static readonly HttpMethod _link = new("LINK");
  private static readonly HttpMethod _unlink = new("UNLINK");
  private static readonly HttpMethod _purge = new("PURGE");
  private static readonly HttpMethod _lock = new("LOCK");
  private static readonly HttpMethod _unlock = new("UNLOCK");
  private static readonly HttpMethod _propfind = new("PROPFIND");
  private static readonly HttpMethod _view = new("VIEW");
  private static readonly HttpMethod _trace = new("TRACE");

  private static readonly Dictionary<string, HttpMethod> _map = new(StringComparer.InvariantCultureIgnoreCase)
  {
    { HttpMethod.Delete.ToString(), HttpMethod.Delete },
    { HttpMethod.Get.ToString(), HttpMethod.Get },
    { HttpMethod.Head.ToString(), HttpMethod.Head },
    { HttpMethod.Options.ToString(), HttpMethod.Options },
    { HttpMethod.Post.ToString(), HttpMethod.Post },
    { HttpMethod.Put.ToString(), HttpMethod.Put },
    { _patch.ToString(), _patch },
    { _copy.ToString(), _copy },
    { _link.ToString(), _link },
    { _unlink.ToString(), _unlink },
    { _purge.ToString(), _purge },
    { _lock.ToString(), _lock },
    { _unlock.ToString(), _unlock },
    { _propfind.ToString(), _propfind },
    { _view.ToString(), _view },
    { _trace.ToString(), _trace },
  };

  public override HttpMethod? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    switch (reader.TokenType)
    {
      case JsonTokenType.String:
        var token = reader.GetString();
        if (token is null || !_map.TryGetValue(token, out var httpMethod))
        {
          throw new JsonException(
            $"Unexpected value {token} when parsing the {nameof(HttpMethod)}.");
        }
        return httpMethod;
      case JsonTokenType.Null:
        return null;
      default:
        throw new JsonException(
          $"Unexpected token {reader.TokenType} when parsing the {nameof(HttpMethod)}.");
    }
  }

  public override void Write(Utf8JsonWriter writer, HttpMethod value, JsonSerializerOptions options)
  {
    var method = value.ToString();

    if (!_map.TryGetValue(method, out var httpMethod))
    {
      throw new JsonException(
        $"Unexpected value {method} when writing the {nameof(HttpMethod)}."); 
    }
    
    writer.WriteStringValue(httpMethod.ToString());
  }
}
