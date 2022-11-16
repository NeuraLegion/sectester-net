using System;

namespace SecTester.Bus.Dispatchers;

public interface MessageSerializer
{
  T? Deserialize<T>(string data);

  object? Deserialize(string data, Type type);

  string Serialize<T>(T data);
}
