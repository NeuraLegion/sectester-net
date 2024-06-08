using System.Collections.Generic;
using System.Linq;
using MessagePack;
using MessagePack.Formatters;

namespace SecTester.Repeater.Internal;

// Headers formatter is to be supporting javascript `undefined` which is treated as null (0xC0)
// https://www.npmjs.com/package/@msgpack/msgpack#messagepack-mapping-table
// https://github.com/msgpack/msgpack/blob/master/spec.md#nil-format

internal class MessagePackHttpHeadersFormatter : IMessagePackFormatter<
  IEnumerable<KeyValuePair<string, IEnumerable<string>>>?
>
{
  public void Serialize(ref MessagePackWriter writer, IEnumerable<KeyValuePair<string, IEnumerable<string>>>? value,
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

      Serialize(ref writer, value);
    }
  }

  private static void Serialize(ref MessagePackWriter writer, IEnumerable<KeyValuePair<string, IEnumerable<string>>> value)
  {
    foreach (var item in value)
    {
      writer.Write(item.Key);

      Serialize(ref writer, item);
    }
  }

  private static void Serialize(ref MessagePackWriter writer, KeyValuePair<string, IEnumerable<string>> item)
  {
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
  }

  public IEnumerable<KeyValuePair<string, IEnumerable<string>>>? Deserialize(ref MessagePackReader reader,
    MessagePackSerializerOptions options)
  {
    if (reader.NextMessagePackType == MessagePackType.Nil)
    {
      reader.ReadNil();
      return null;
    }

    if (reader.NextMessagePackType != MessagePackType.Map)
    {
      throw new MessagePackSerializationException($"Unrecognized code: 0x{reader.NextCode:X2} but expected to be a map or null");
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

  private static IEnumerable<KeyValuePair<string, IEnumerable<string>>> DeserializeMap(ref MessagePackReader reader, int length,
    MessagePackSerializerOptions options)
  {
    var result = new List<KeyValuePair<string, IEnumerable<string>>>(length);

    for (var i = 0 ; i < length ; i++)
    {
      var key = DeserializeString(ref reader);

      result.Add(new KeyValuePair<string, IEnumerable<string>>(
        key,
        DeserializeValue(ref reader, options)
      ));
    }

    return result;
  }

  private static IEnumerable<string> DeserializeArray(ref MessagePackReader reader, int length, MessagePackSerializerOptions options)
  {
    var result = new List<string>(length);

    options.Security.DepthStep(ref reader);

    try
    {
      for (var i = 0 ; i < length ; i++)
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

  private static IEnumerable<string> DeserializeValue(ref MessagePackReader reader, MessagePackSerializerOptions options)
  {
    switch (reader.NextMessagePackType)
    {
      case MessagePackType.Nil:
        reader.ReadNil();
        return new List<string>();
      case MessagePackType.String:
        return new List<string> { DeserializeString(ref reader) };
      case MessagePackType.Array:
        return DeserializeArray(ref reader, reader.ReadArrayHeader(), options);
      default:
        throw new MessagePackSerializationException(
          $"Unrecognized code: 0x{reader.NextCode:X2} but expected to be either a string or an array.");
    }
  }

  private static string DeserializeString(ref MessagePackReader reader)
  {
    if (reader.NextMessagePackType != MessagePackType.String)
    {
      throw new MessagePackSerializationException($"Unrecognized code: 0x{reader.NextCode:X2} but expected to be a string.");
    }

    return reader.ReadString() ?? string.Empty;
  }
}
