using MessagePack;
using SecTester.Repeater.Bus.Formatters;

namespace SecTester.Repeater.Tests.Bus.Formatters;

public sealed class MessagePackHttpMethodFormatterTests
{
  [MessagePackObject]
  public record HttpMethodDto
  {
    [Key("method")]
    [MessagePackFormatter(typeof(MessagePackHttpMethodFormatter))]
    public HttpMethod? Method { get; set; }
  }

  private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard;

  private static IEnumerable<HttpMethodDto>
    Fixtures =>
    new[]
    {
      new HttpMethodDto
      {
        Method = null
      },
      new HttpMethodDto
      {
        Method = HttpMethod.Get
      },
      new HttpMethodDto
      {
        Method = new HttpMethod("PROPFIND")
      }
    };

  public static IEnumerable<object?[]> SerializeDeserializeFixtures => Fixtures
    .Select((x) => new object?[]
    {
      x, x
    });

  [Theory]
  [MemberData(nameof(SerializeDeserializeFixtures))]
  public void HttpMethodMessagePackFormatter_Deserialize_ShouldCorrectlyDeserializePreviouslySerializedValue(
    HttpMethodDto input,
    HttpMethodDto expected)
  {
    // arrange
    var serialized = MessagePackSerializer.Serialize(input, Options);

    // act
    var result = MessagePackSerializer.Deserialize<HttpMethodDto>(serialized, Options);

    // assert
    result.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void HttpMethodMessagePackFormatter_Deserialize_ShouldCorrectlyHandleMissingValue()
  {
    // arrange
    var binary = MessagePackSerializer.ConvertFromJson("{}", Options);

    // act
    var result = MessagePackSerializer.Deserialize<HttpMethodDto>(binary, Options);

    // assert
    result.Should().BeEquivalentTo(new HttpMethodDto
    {
      Method = null
    });
  }
}
