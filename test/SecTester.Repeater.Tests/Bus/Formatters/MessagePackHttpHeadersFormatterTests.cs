using MessagePack;
using SecTester.Repeater.Bus.Formatters;

namespace SecTester.Repeater.Tests.Bus.Formatters;

public sealed class MessagePackHttpHeadersFormatterTests
{
  [MessagePackObject]
  public record HttpHeadersDto
  {
    [Key("headers")]
    [MessagePackFormatter(typeof(MessagePackHttpHeadersFormatter))]
    public IEnumerable<KeyValuePair<string, IEnumerable<string>>>? Headers { get; set; }
  }

  private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard;

  private static IEnumerable<
      HttpHeadersDto>
    Fixtures =>
    new[]
    {
      new HttpHeadersDto
      {
        Headers = null
      },
      new HttpHeadersDto
      {
        Headers = new List<KeyValuePair<string, IEnumerable<string>>>()
      },

      new HttpHeadersDto
      {
        Headers = new List<KeyValuePair<string, IEnumerable<string>>>
        {
          new("content-type", new List<string> { "application/json" }),
          new("cache-control", new List<string> { "no-cache", "no-store" })
        }
      }
    };

  public static readonly IEnumerable<object[]> WrongFormatFixtures = new List<object[]>
  {
    new object[]
    {
      "{\"headers\":5}",
    },
    new object[]
    {
      "{\"headers\":[]}",
    },
    new object[]
    {
      "{\"headers\":{\"content-type\":{\"foo\"}:{\"bar\"}}}",
    }
  };

  public static IEnumerable<object?[]> SerializeDeserializeFixtures => Fixtures
    .Select((x) => new object?[]
    {
      x, x
    });

  [Theory]
  [MemberData(nameof(SerializeDeserializeFixtures))]
  public void HttpHeadersMessagePackFormatter_Deserialize_ShouldCorrectlyDeserializePreviouslySerializedValue(
    HttpHeadersDto input,
    HttpHeadersDto expected)
  {
    // arrange
    var serialized = MessagePackSerializer.Serialize(input, Options);

    // act
    var result = MessagePackSerializer.Deserialize<HttpHeadersDto>(serialized, Options);

    // assert
    result.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void HttpHeadersMessagePackFormatter_Deserialize_ShouldCorrectlyHandleMissingValue()
  {
    // arrange
    var binary = MessagePackSerializer.ConvertFromJson("{}", Options);

    // act
    var result = MessagePackSerializer.Deserialize<HttpHeadersDto>(binary, Options);

    // assert
    result.Should().BeEquivalentTo(new HttpHeadersDto
    {
      Headers = null
    });
  }

  [Theory]
  [MemberData(nameof(WrongFormatFixtures))]
  public void HttpHeadersMessagePackFormatter_Deserialize_ShouldThrow(
    string input)
  {
    // arrange
    var binary = MessagePackSerializer.ConvertFromJson(input, Options);

    // act
    var act = () => MessagePackSerializer.Deserialize<HttpHeadersDto>(binary, Options);

    // assert
    act.Should().Throw<MessagePackSerializationException>().WithMessage("Failed to deserialize SecTester.Repeater.Tests.Bus.Formatters.MessagePackHttpHeadersFormatterTests+HttpHeadersDto value.");
  }
}
