using System.Globalization;
using System.Net.Http;
using MessagePack;
using MessagePack.Formatters;

namespace SecTester.Repeater.Bus.Formatters;

public class MessagePackHttpMethodFormatter : IMessagePackFormatter<HttpMethod?>
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
    switch (reader.NextMessagePackType)
    {
      case MessagePackType.Nil:
        return null;
      case MessagePackType.String:
        var method = reader.ReadString();
        return string.IsNullOrWhiteSpace(method) ? null : new HttpMethod(method);
      default:
        throw new MessagePackSerializationException(string.Format(CultureInfo.InvariantCulture,
          "Unrecognized code: 0x{0:X2} but expected to be either a string or null.", reader.NextCode));
    }
  }
}
