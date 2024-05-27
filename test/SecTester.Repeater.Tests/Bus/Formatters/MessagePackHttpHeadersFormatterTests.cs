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

  public static readonly IEnumerable<object[]> Fixtures = new List<object[]>()
  {
    new object[]
    {
      new HttpHeadersDto
      {
        Headers = null
      }
    },
    new object[]
    {
      new HttpHeadersDto
      {
        Headers = new List<KeyValuePair<string, IEnumerable<string>>>()
      }
    },
    new object[]
    {
      new HttpHeadersDto
      {
        Headers = new List<KeyValuePair<string, IEnumerable<string>>>
        {
          new("content-type", new List<string> { "application/json" }),
          new("cache-control", new List<string> { "no-cache", "no-store" })
        }
      }
    }
  };

  public static readonly IEnumerable<object[]> WrongValueFixtures = new List<object[]>()
  {
    new object[]
    {
      "{\"headers\":5}"
    },
    new object[] { "{\"headers\":[]}" },
    new object[] { "{\"headers\":{\"content-type\":{\"foo\"}:{\"bar\"}}}" },
    new object[] { "{\"headers\":{\"content-type\":1}}" },
    new object[] { "{\"headers\":{\"content-type\":[null]}}" },
    new object[] { "{\"headers\":{\"content-type\":[1]}}" }
  };

  [Theory]
  [MemberData(nameof(Fixtures))]
  public void HttpHeadersMessagePackFormatter_Deserialize_ShouldCorrectlyDeserializePreviouslySerializedValue(
    HttpHeadersDto input)
  {
    // arrange
    var serialized = MessagePackSerializer.Serialize(input, Options);

    // act
    var result = MessagePackSerializer.Deserialize<HttpHeadersDto>(serialized, Options);

    // assert
    result.Should().BeEquivalentTo(input);
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
  [MemberData(nameof(WrongValueFixtures))]
  public void HttpHeadersMessagePackFormatter_Deserialize_ShouldThrowWhenDataHasWrongValue(
    string input)
  {
    // arrange
    var binary = MessagePackSerializer.ConvertFromJson(input, Options);

    // act
    var act = () => MessagePackSerializer.Deserialize<HttpHeadersDto>(binary, Options);

    // assert
    act.Should().Throw<MessagePackSerializationException>().WithMessage(
      "Failed to deserialize*");
  }
}
