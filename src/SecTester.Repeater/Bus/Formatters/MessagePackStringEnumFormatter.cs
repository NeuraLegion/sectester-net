using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MessagePack;
using MessagePack.Formatters;

namespace SecTester.Repeater.Bus.Formatters;

// ADHOC: MessagePack-CSharp prohibits declaration of IMessagePackFormatter<T> requesting to use System.Enum instead, refer to formatter interface argument type check
// https://github.com/MessagePack-CSharp/MessagePack-CSharp/blob/db2320b3338735c9266110bbbfffe63f17dfdf46/src/MessagePack.UnityClient/Assets/Scripts/MessagePack/Resolvers/DynamicObjectResolver.cs#L623

public class MessagePackStringEnumFormatter<T> : IMessagePackFormatter<Enum>
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

  private static readonly Dictionary<string, T> StringToEnum = EnumToString.ToDictionary(x => x.Value, x => x.Key);

  public void Serialize(ref MessagePackWriter writer, Enum value, MessagePackSerializerOptions options)
  {
    if (!EnumToString.TryGetValue((T)value, out var stringValue))
    {
      throw new MessagePackSerializationException($"No string representation found for {value}");
    }

    writer.Write(stringValue);
  }

  public Enum Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
  {

    var stringValue = reader.ReadString();

    if (!StringToEnum.TryGetValue(stringValue, out var enumValue))
    {
      throw new MessagePackSerializationException($"Unable to parse '{stringValue}' to {typeof(T).Name}.");
    }

    return enumValue;
  }
}
