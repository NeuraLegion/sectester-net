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
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters =
    {
      new JsonHttpMethodEnumerationStringConverter(), new JsonStringEnumMemberConverter(SnakeCaseNamingPolicy.Instance, false)
    }
  };

  public static T? Deserialize<T>(string data)
  {
    return (T?)Deserialize(data, typeof(T));
  }

  public static object? Deserialize(string data, Type type)
  {
    return JsonSerializer.Deserialize(data, type, Options);
  }

  public static string Serialize<T>(T data)
  {
    return JsonSerializer.Serialize(data, Options);
  }
}

