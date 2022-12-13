using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SecTester.Bus.Internal;

namespace SecTester.Bus.Dispatchers;

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
      new JsonStringEnumMemberConverter(SnakeCaseNamingPolicy.Instance, false),
      new HeadersConverter()
    }
  };

  public static T? Deserialize<T>(string data) => (T?)Deserialize(data, typeof(T));

  public static object? Deserialize(string data, Type type) => JsonSerializer.Deserialize(data, type, Options);

  public static string Serialize<T>(T data) => JsonSerializer.Serialize(data, Options);
}
