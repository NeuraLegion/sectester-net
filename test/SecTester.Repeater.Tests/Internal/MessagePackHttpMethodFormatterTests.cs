using MessagePack;
using MessagePack.Resolvers;
using SecTester.Repeater.Internal;

namespace SecTester.Repeater.Tests.Internal;

public sealed class MessagePackHttpMethodFormatterTests
{
  private static readonly MessagePackSerializerOptions Options = new(
   CompositeResolver.Create(
     CompositeResolver.Create(new MessagePackHttpMethodFormatter()),
     BuiltinResolver.Instance
   )
 );

  public static readonly IEnumerable<object[]> Fixture = new List<object[]>
  {
    new object[] { "DELETE", HttpMethod.Delete },
    new object[] { "GET", HttpMethod.Get },
    new object[] { "HEAD", HttpMethod.Head },
    new object[] { "OPTIONS", HttpMethod.Options },
    new object[] { "PATCH", HttpMethod.Patch },
    new object[] { "POST", HttpMethod.Post },
    new object[] { "PUT", HttpMethod.Put },
    new object[] { "TRACE", HttpMethod.Trace },
    new object[] { "COPY", new HttpMethod("COPY") },
    new object[] { "LINK", new HttpMethod("LINK") },
    new object[] { "UNLINK", new HttpMethod("UNLINK") },
    new object[] { "PURGE", new HttpMethod("PURGE") },
    new object[] { "LOCK", new HttpMethod("LOCK") },
    new object[] { "UNLOCK", new HttpMethod("UNLOCK") },
    new object[] { "PROPFIND", new HttpMethod("PROPFIND") },
    new object[] { "VIEW", new HttpMethod("VIEW") }
  };

  [Theory]
  [MemberData(nameof(Fixture))]
  public void HttpMethodMessagePackFormatter_Deserialize_ShouldCorrectlyDeserializeHttpMethods(
    string input, HttpMethod expected)
  {
    // arrange
    var binary = MessagePackSerializer.Serialize<string>(input, Options);

    // act
    var result = MessagePackSerializer.Deserialize<HttpMethod>(binary, Options);

    // assert
    result.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void HttpMethodMessagePackFormatter_Deserialize_ShouldHandleNull()
  {
    // arrange
    var binary = MessagePackSerializer.Serialize<string>(null, Options);

    // act
    var result = MessagePackSerializer.Deserialize<HttpMethod>(binary, Options);
    // assert
    result.Should().BeNull();
  }
}
