using MessagePack;
using SecTester.Repeater.Internal;
using SocketIO.Core;
using SocketIO.Serializer.MessagePack;

namespace SecTester.Repeater.Tests.Bus;

public class IncomingRequestTests
{
  private static readonly MessagePackSerializerOptions Options = DefaultMessagePackSerializerOptions.Instance;

  private static readonly IEnumerable<IncomingRequest> ValidFixtures = new[]
  {
    new IncomingRequest(new Uri("http://foo.bar/1"))
    {
      Protocol = Protocol.Http,
      Method = new HttpMethod("PROPFIND"),
      Headers = new List<KeyValuePair<string, IEnumerable<string>>>
      {
        new("content-type", new List<string> { "application/json" }),
        new("cache-control", new List<string> { "no-cache", "no-store" })
      },
      Body = "{\"foo\":\"bar\"}"
    },
    new IncomingRequest(new Uri("http://foo.bar/2"))
    {
      Protocol = Protocol.Http,
      Headers = new List<KeyValuePair<string, IEnumerable<string>>>()
    }
  };

  public static IEnumerable<object?[]> CreateInstanceFixtures => ValidFixtures
    .Select((x) => new object?[]
    {
      x
    });

  [Theory]
  [MemberData(nameof(CreateInstanceFixtures))]
  public void IncomingRequest_FromDictionary_ShouldCreateInstance(IncomingRequest input)
  {
    // arrange
    var serializer = new SocketIOMessagePackSerializer(Options);

    var serialized = serializer.Serialize(EngineIO.V4, "request", 1, "/some", new object[] { input }).First();

    var deserializedPackMessage = MessagePackSerializer.Deserialize<PackMessage>(serialized.Binary, Options);

    var deserializedDictionary = serializer.Deserialize<Dictionary<object, object>>(deserializedPackMessage, 1);

    // act
    var result = IncomingRequest.FromDictionary(deserializedDictionary);

    // assert
    result.Should().BeEquivalentTo(input);
  }

  [Fact]
  public void IncomingRequest_FromDictionary_ShouldThrowWhenProtocolIsInvalid()
  {
    // arrange
    var packJson =
      "{\"type\":2,\"data\":[\"request\",{\"protocol\":\"http:\",\"headers\":{\"content-type\":\"application/json\",\"cache-control\":[\"no-cache\",\"no-store\"]},\"body\":\"{\\\"foo\\\":\\\"bar\\\"}\",\"method\":\"PROPFIND\"}],\"options\":{\"compress\":true},\"id\":1,\"nsp\":\"/some\"}";

    var serializer = new SocketIOMessagePackSerializer(Options);

    var deserializedPackMessage = MessagePackSerializer.Deserialize<PackMessage>(MessagePackSerializer.ConvertFromJson(packJson), Options);

    var deserializedDictionary = serializer.Deserialize<Dictionary<object, object>>(deserializedPackMessage, 1);

    // act
    var act = () => IncomingRequest.FromDictionary(deserializedDictionary);

    // assert
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void IncomingRequest_FromDictionary_ShouldAssignDefaultPropertyValues()
  {
    // arrange
    var packJson =
      "{\"type\":2,\"data\":[\"request\",{\"url\":\"https://foo.bar/1\"}],\"options\":{\"compress\":true},\"id\":1,\"nsp\":\"/some\"}";

    var serializer = new SocketIOMessagePackSerializer(Options);

    var deserializedPackMessage = MessagePackSerializer.Deserialize<PackMessage>(MessagePackSerializer.ConvertFromJson(packJson), Options);

    var deserializedDictionary = serializer.Deserialize<Dictionary<object, object>>(deserializedPackMessage, 1);

    // act
    var result = IncomingRequest.FromDictionary(deserializedDictionary);

    // assert
    result.Should().BeEquivalentTo(new IncomingRequest(new Uri("https://foo.bar/1")));
  }

  [Fact]
  public void IncomingRequest_FromDictionary_ShouldParseProtocolValue()
  {
    // arrange
    var packJson =
      "{\"type\":2,\"data\":[\"request\",{\"protocol\":\"http\",\"url\":\"https://foo.bar/1\"}],\"options\":{\"compress\":true},\"id\":1,\"nsp\":\"/some\"}";

    var serializer = new SocketIOMessagePackSerializer(Options);

    var deserializedPackMessage = MessagePackSerializer.Deserialize<PackMessage>(MessagePackSerializer.ConvertFromJson(packJson), Options);

    var deserializedDictionary = serializer.Deserialize<Dictionary<object, object>>(deserializedPackMessage, 1);

    // act
    var result = IncomingRequest.FromDictionary(deserializedDictionary);

    // assert
    result.Should().BeEquivalentTo(new IncomingRequest(new Uri("https://foo.bar/1")) { Protocol = Protocol.Http });
  }
}
