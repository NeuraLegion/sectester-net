using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MessagePack;
using MessagePack.Formatters;

namespace SecTester.Repeater.Internal;

internal class MessagePackStringEnumMemberFormatter<T> : IMessagePackFormatter<T>
  where T : Enum
{
  private static readonly Dictionary<T, string> EnumToString = typeof(T)
    .GetFields(BindingFlags.Public | BindingFlags.Static)
    .Select(field => new
    {
      Value = (T)field.GetValue(null),
      StringValue = field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? field.Name
    })
    .ToDictionary(x => x.Value, x => x.StringValue);

  private readonly Dictionary<string, T> _casedStringToEnum;
  private readonly Dictionary<T, string> _casedEnumToString;

  public MessagePackStringEnumMemberFormatter(MessagePackNamingPolicy namingPolicy)
  {
    this._casedEnumToString = EnumToString.ToDictionary(x => x.Key, x => namingPolicy.ConvertName(x.Value));
    this._casedStringToEnum = EnumToString.ToDictionary(x => namingPolicy.ConvertName(x.Value), x => x.Key);
  }

  public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
  {
    if (!_casedEnumToString.TryGetValue(value, out var stringValue))
    {
      throw new MessagePackSerializationException($"No string representation found for {value}");
    }

    writer.Write(stringValue);
  }

  public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
  {
    var stringValue = reader.ReadString();

    if (null == stringValue || !_casedStringToEnum.TryGetValue(stringValue, out var enumValue))
    {
      throw new MessagePackSerializationException($"Unable to parse '{stringValue}' to {typeof(T).Name}.");
    }

    return enumValue;
  }
}
