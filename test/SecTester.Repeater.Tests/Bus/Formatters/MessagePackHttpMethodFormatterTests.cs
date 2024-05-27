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

  public static readonly IEnumerable<object[]> Fixtures = new List<object[]>()
  {
    new object[]
    {
      new HttpMethodDto
      {
        Method = null
      }
    },
    new object[]
    {
      new HttpMethodDto
      {
        Method = HttpMethod.Get
      }
    },
    new object[]
    {
      new HttpMethodDto
      {
        Method = new HttpMethod("PROPFIND")
      }
    }
  };

  [Theory]
  [MemberData(nameof(Fixtures))]
  public void HttpMethodMessagePackFormatter_Deserialize_ShouldCorrectlyDeserializePreviouslySerializedValue(
    HttpMethodDto input)
  {
    // arrange
    var serialized = MessagePackSerializer.Serialize(input, Options);

    // act
    var result = MessagePackSerializer.Deserialize<HttpMethodDto>(serialized, Options);

    // assert
    result.Should().BeEquivalentTo(input);
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
