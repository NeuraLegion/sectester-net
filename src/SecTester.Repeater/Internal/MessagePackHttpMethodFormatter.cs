using System.Net.Http;
using MessagePack;
using MessagePack.Formatters;

namespace SecTester.Repeater.Internal;

internal class MessagePackHttpMethodFormatter : IMessagePackFormatter<HttpMethod?>
{
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

    if (token is null || !HttpMethods.Items.TryGetValue(token, out var method))
    {
      throw new MessagePackSerializationException(
        $"Unexpected value {token} when parsing the {nameof(HttpMethod)}.");
    }

    return method;
  }
}
