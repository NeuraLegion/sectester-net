using System;
using System.Text.Json;

namespace SecTester.Bus.Dispatchers;

public class DefaultMessageSerializer : MessageSerializer
{
  private JsonSerializerOptions Options => new()
  {
    IncludeFields = true,
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public T? Deserialize<T>(string data)
  {
    return (T?)Deserialize(data, typeof(T));
  }

  public object? Deserialize(string data, Type type)
  {
    return JsonSerializer.Deserialize(data, type, Options);
  }

  public string Serialize<T>(T data)
  {
    return JsonSerializer.Serialize(data, Options);
  }
}
