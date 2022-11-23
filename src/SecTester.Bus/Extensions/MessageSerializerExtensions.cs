using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using SecTester.Bus.Dispatchers;

namespace SecTester.Bus.Extensions;

public static class MessageSerializerExtensions
{
  public static HttpContent SerializeJsonContent<T>(this MessageSerializer messageSerializer, T data)
  {
    return new StringContent(messageSerializer.Serialize(data), Encoding.UTF8, "application/json");
  }
}
