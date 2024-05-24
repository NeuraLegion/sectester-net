using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MessagePack;
using MessagePack.Formatters;

namespace SecTester.Repeater.Bus.Formatters;

// Headers formatter is to be support javascript `undefined` which is treated as null (0xC0)
// https://www.npmjs.com/package/@msgpack/msgpack#messagepack-mapping-table
// https://github.com/msgpack/msgpack/blob/master/spec.md#nil-format

public class MessagePackHttpHeadersFormatter: IMessagePackFormatter<
  IEnumerable<KeyValuePair<string, IEnumerable<string>>>
>
{
  public MessagePackHttpHeadersFormatter()
  {
    // noop
  }

  public void Serialize(ref MessagePackWriter writer, IEnumerable<KeyValuePair<string, IEnumerable<string>>> value,
    MessagePackSerializerOptions options)
  {

    if (value == null)
    {
      writer.WriteNil();
    }
    else
    {
      var count = value.Count();

      writer.WriteMapHeader(count);

      SerializeMap(ref writer, value);
    }
  }



  public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Deserialize(ref MessagePackReader reader,
    MessagePackSerializerOptions options)
  {
    switch (reader.NextMessagePackType)
    {
      case MessagePackType.Nil:
        return null;
      case MessagePackType.Map:
        break;
      default:
        throw new MessagePackSerializationException(string.Format(CultureInfo.InvariantCulture,
          "Unrecognized code: 0x{0:X2} but expected to be a map or null", reader.NextCode));
    }

    var length = reader.ReadMapHeader();

    options.Security.DepthStep(ref reader);

    try
    {
      return DeserializeMap(ref reader, length, options);
    }
    finally
    {
      reader.Depth--;
    }
  }

  private static void SerializeMap(ref MessagePackWriter writer, IEnumerable<KeyValuePair<string, IEnumerable<string>>> value)
  {

    foreach (var item in value)
    {
      writer.Write(item.Key);

      var headersCount = item.Value.Count();

      if (headersCount == 1)
      {
        writer.Write(item.Value.First());
      }
      else
      {
        writer.WriteArrayHeader(headersCount);

        foreach (var subItem in item.Value)
        {
          writer.Write(subItem);
        }
      }
    };
  }


  private static List<KeyValuePair<string, IEnumerable<string>>> DeserializeMap(ref MessagePackReader reader, int length, MessagePackSerializerOptions options)
  {
    var result = new List<KeyValuePair<string, IEnumerable<string>>>(length);

    for ( int i = 0 ; i < length ; i++ )
    {
      var key = DeserializeString(ref reader);

      switch (reader.NextMessagePackType)
      {
        case MessagePackType.String:
          result.Add(new KeyValuePair<string, IEnumerable<string>>(key, new List<string>{DeserializeString(ref reader)}));
          break;
        case MessagePackType.Array:
          result.Add(new KeyValuePair<string, IEnumerable<string>>(key, DeserializeArray(ref reader, reader.ReadArrayHeader(), options)));
          break;
        default:
          throw new MessagePackSerializationException(string.Format(CultureInfo.InvariantCulture, "Unrecognized code: 0x{0:X2} but expected to be either a string or an array.", reader.NextCode));
      }
    }


    return result;
  }

  private static IEnumerable<string> DeserializeArray(ref MessagePackReader reader, int length, MessagePackSerializerOptions options)
  {
    var result = new List<string>(length);

    if (length == 0)
    {
      return result;
    }

    options.Security.DepthStep(ref reader);
    try
    {
      for ( int i = 0 ; i < length ; i++ )
      {
        result.Add(DeserializeString(ref reader));
      }
    }
    finally
    {
      reader.Depth--;
    }

    return result;
  }

  private static string DeserializeString(ref MessagePackReader reader)
  {
    if (reader.NextMessagePackType != MessagePackType.String)
    {
      throw new MessagePackSerializationException(string.Format(CultureInfo.InvariantCulture, "Unrecognized code: 0x{0:X2} but expected to be a string.", reader.NextCode));
    }

    var value = reader.ReadString();

    if (null == value)
    {
      throw new MessagePackSerializationException(string.Format(CultureInfo.InvariantCulture, "Nulls are not allowed."));
    }

    return value;
  }
}
