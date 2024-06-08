using MessagePack;
using MessagePack.Resolvers;
using SecTester.Repeater.Internal;

namespace SecTester.Repeater.Tests.Internal;

public sealed class MessagePackHttpHeadersFormatterTests
{
  private static readonly MessagePackSerializerOptions Options = new(
    CompositeResolver.Create(
      CompositeResolver.Create(new MessagePackHttpHeadersFormatter()),
      BuiltinResolver.Instance
    )
  );

  public static readonly IEnumerable<object?[]> Fixtures = new List<object?[]>()
  {
    new object?[]
    {
      null
    },
    new object[]
    {
      Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>()
    },
    new object?[]
    {
      new List<KeyValuePair<string, IEnumerable<string>>>
        {
          new("content-type", new List<string> { "application/json" }),
          new("cache-control", new List<string> { "no-cache", "no-store" })
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
    IEnumerable<KeyValuePair<string, IEnumerable<string>>>? input)
  {
    // arrange
    var serialized = MessagePackSerializer.Serialize(input, Options);

    // act
    var result = MessagePackSerializer.Deserialize<IEnumerable<KeyValuePair<string, IEnumerable<string>>>>(serialized, Options);

    // assert
    result.Should().BeEquivalentTo(input);
  }
}
