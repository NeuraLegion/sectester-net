using MessagePack;
using SecTester.Repeater.Internal;

namespace SecTester.Repeater.Tests.Internal;

public sealed class DefaultMessagePackSerializerOptionsTests
{
  [MessagePackObject]
  public record TestDto
  {
    [Key("protocol")]
    public Protocol Protocol { get; set; } = Protocol.Http;

    [Key("method")]
    public HttpMethod Method { get; set; } = HttpMethod.Delete;

    private IEnumerable<KeyValuePair<string, IEnumerable<string>>>? _headers;

    [Key("headers")]
    public IEnumerable<KeyValuePair<string, IEnumerable<string>>>? Headers {
      get => this._headers;
      set => this._headers = null != value ? new List<KeyValuePair<string, IEnumerable<string>>>(value) : value;
    }
  }

  private static readonly MessagePackSerializerOptions Options = DefaultMessagePackSerializerOptions.Instance;

  private static IEnumerable<
      TestDto>
    Fixtures =>
    new []
    {
      new TestDto
      {
        Protocol  = Protocol.Http,
        Method  = HttpMethod.Put,
        Headers = new List<KeyValuePair<string, IEnumerable<string>>>
        {
          new("content-type", new List<string> { "application/json" }),
          new("cache-control", new List<string> { "no-cache", "no-store" })
        }
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
    TestDto input,
    TestDto expected)
  {
    // arrange
    var serialized = MessagePackSerializer.Serialize(input, Options);

    // act
    var result = MessagePackSerializer.Deserialize<TestDto>(serialized, Options);

    // assert
    result.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void HttpHeadersMessagePackFormatter_Deserialize_ShouldCorrectlyHandleMissingValue()
  {
    // arrange
    var binary = MessagePackSerializer.ConvertFromJson("{}", Options);

    // act
    var result = MessagePackSerializer.Deserialize<TestDto>(binary, Options);

    // assert
    result.Should().BeEquivalentTo(new TestDto
    {
      Headers = null
    });
  }
}
