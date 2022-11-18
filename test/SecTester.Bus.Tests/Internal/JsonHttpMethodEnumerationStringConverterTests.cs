namespace SecTester.Bus.Tests.Internal;

public class JsonHttpMethodEnumerationStringConverterTests
{
  private readonly DefaultMessageSerializer _sut = new();

  public static readonly IEnumerable<object[]> HttpMethodEnumerable = new List<object[]>()
  {
    new object[] { HttpMethod.Delete, @"""DELETE""", typeof(HttpMethod) },
    new object[] { HttpMethod.Get, @"""GET""", typeof(HttpMethod) },
    new object[] { HttpMethod.Head, @"""HEAD""", typeof(HttpMethod) },
    new object[] { HttpMethod.Options, @"""OPTIONS""", typeof(HttpMethod) },
    new object[] { HttpMethod.Patch, @"""PATCH""", typeof(HttpMethod) },
    new object[] { HttpMethod.Post, @"""POST""", typeof(HttpMethod) },
    new object[] { HttpMethod.Put, @"""PUT""", typeof(HttpMethod) },
    new object[] { HttpMethod.Trace, @"""TRACE""", typeof(HttpMethod) },
    new object[] { new HttpMethod("COPY"), @"""COPY""", typeof(HttpMethod) },
    new object[] { new HttpMethod("LINK"), @"""LINK""", typeof(HttpMethod) },
    new object[] { new HttpMethod("UNLINK"), @"""UNLINK""", typeof(HttpMethod) },
    new object[] { new HttpMethod("PURGE"), @"""PURGE""", typeof(HttpMethod) },
    new object[] { new HttpMethod("LOCK"), @"""LOCK""", typeof(HttpMethod) },
    new object[] { new HttpMethod("UNLOCK"), @"""UNLOCK""", typeof(HttpMethod) },
    new object[] { new HttpMethod("PROPFIND"), @"""PROPFIND""", typeof(HttpMethod) },
    new object[] { new HttpMethod("VIEW"), @"""VIEW""", typeof(HttpMethod) }
  };

  [Theory]
  [MemberData(nameof(HttpMethodEnumerable))]
  public void Serialize_GivenEnumValue_ReturnString(object value, string valueString, Type type)
  {
    // act
    var data = _sut.Serialize(value);

    // assert
    data.Should().Be(valueString);
  }

  [Theory]
  [MemberData(nameof(HttpMethodEnumerable))]
  public void Deserialize_GivenString_ReturnEnumValue(object value, string valueString, Type type)
  {
    // act
    var result = _sut.Deserialize(valueString, type);

    // assert
    result.Should().Be(value);
  }
  
  [Fact]
  public void  Deserialize_GivenMissingFieldInput_ReturnObject()
  {
    // act
    var result = _sut.Deserialize<Tuple<HttpMethod?>>("{}");

    // assert
    result.Should().BeOfType<Tuple<HttpMethod?>>();
    result!.Item1.Should().BeNull();
  }
}
