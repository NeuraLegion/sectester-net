using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SecTester.Repeater.Internal;

namespace SecTester.Repeater.Dispatchers;

public static class MessageSerializer
{
  private static readonly JsonSerializerOptions Options = new()
  {
    IncludeFields = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters =
    {
      new JsonHttpMethodEnumerationStringConverter(),
      new JsonStringEnumMemberConverter(JsonSnakeCaseNamingPolicy.Instance, false),
      new JsonHeadersConverter()
    }
  };

  public static T? Deserialize<T>(string data) => (T?)Deserialize(data, typeof(T));

  public static object? Deserialize(string data, Type type) => JsonSerializer.Deserialize(data, type, Options);

  public static string Serialize<T>(T data) => JsonSerializer.Serialize(data, Options);
}
