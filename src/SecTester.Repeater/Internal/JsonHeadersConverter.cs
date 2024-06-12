using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecTester.Repeater.Internal;

internal sealed class JsonHeadersConverter : JsonConverter<IEnumerable<KeyValuePair<string, IEnumerable<string>>>>
{
  private static readonly Type Type = typeof(IEnumerable<KeyValuePair<string, IEnumerable<string>>>);
  private readonly bool _keepSingleElementSequence;

  internal JsonHeadersConverter(bool keepSingleElementSequence = false)
  {
    _keepSingleElementSequence = keepSingleElementSequence;
  }

  public override bool CanConvert(Type typeToConvert) => typeToConvert.IsGenericType && typeToConvert == Type;

  public override IEnumerable<KeyValuePair<string, IEnumerable<string>>> Read(ref Utf8JsonReader reader, Type typeToConvert,
    JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.StartObject)
    {
      throw new JsonException();
    }

    var result = new List<KeyValuePair<string, IEnumerable<string>>>();

    while (reader.Read())
    {
      if (reader.TokenType == JsonTokenType.EndObject)
      {
        break;
      }

      result.Add(DeserializeKeyValuePair(ref reader, options));
    }

    return result;
  }

  private static KeyValuePair<string, IEnumerable<string>> DeserializeKeyValuePair(ref Utf8JsonReader reader, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.PropertyName)
    {
      throw new JsonException();
    }

    var key = reader.GetString()!;
    reader.Read();

    var value = DeserializeValue(ref reader, options) ?? Array.Empty<string>();

    return new KeyValuePair<string, IEnumerable<string>>(key, value);
  }

  private static IEnumerable<string>? DeserializeValue(ref Utf8JsonReader reader, JsonSerializerOptions options) =>
    reader.TokenType switch
    {
      JsonTokenType.StartArray => JsonSerializer.Deserialize<IEnumerable<string>>(ref reader, options),
      _ => new List<string>
      {
        JsonSerializer.Deserialize<string>(ref reader, options)!
      }
    };

  public override void Write(Utf8JsonWriter writer, IEnumerable<KeyValuePair<string, IEnumerable<string>>> value,
    JsonSerializerOptions options)
  {
    writer.WriteStartObject();

    foreach (var kvp in value)
    {
      SerializeKeyValuePair(ref writer, options, kvp);
    }

    writer.WriteEndObject();
  }

  private void SerializeKeyValuePair(ref Utf8JsonWriter writer, JsonSerializerOptions options,
    KeyValuePair<string, IEnumerable<string>> kvp)
  {
    var propertyName = kvp.Key;

    writer.WritePropertyName(propertyName);

    SerializeValue(ref writer, options, kvp.Value);
  }

  private void SerializeValue(ref Utf8JsonWriter writer, JsonSerializerOptions options, IEnumerable<string> value)
  {
    if (value.Count() > 1 || _keepSingleElementSequence)
    {
      JsonSerializer.Serialize(writer, value, options);
    }
    else
    {
      JsonSerializer.Serialize(writer, value.FirstOrDefault(), options);
    }
  }
}
