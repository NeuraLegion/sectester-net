using SecTester.Core.Tests.Fixtures;


namespace SecTester.Core.Tests.Dispatchers;

public class MessageSerializerTests
{

  public static readonly IEnumerable<object[]> HttpMethodEnumerable = new List<object[]>
  {
    new object[] { HttpMethod.Delete, @"""DELETE""" },
    new object[] { HttpMethod.Get, @"""GET""" },
    new object[] { HttpMethod.Head, @"""HEAD""" },
    new object[] { HttpMethod.Options, @"""OPTIONS""" },
    new object[] { HttpMethod.Patch, @"""PATCH""" },
    new object[] { HttpMethod.Post, @"""POST""" },
    new object[] { HttpMethod.Put, @"""PUT""" },
    new object[] { HttpMethod.Trace, @"""TRACE""" },
    new object[] { new HttpMethod("COPY"), @"""COPY""" },
    new object[] { new HttpMethod("LINK"), @"""LINK""" },
    new object[] { new HttpMethod("UNLINK"), @"""UNLINK""" },
    new object[] { new HttpMethod("PURGE"), @"""PURGE""" },
    new object[] { new HttpMethod("LOCK"), @"""LOCK""" },
    new object[] { new HttpMethod("UNLOCK"), @"""UNLOCK""" },
    new object[] { new HttpMethod("PROPFIND"), @"""PROPFIND""" },
    new object[] { new HttpMethod("VIEW"), @"""VIEW""" }
  };

  public static readonly IEnumerable<object[]> Headers = new List<object[]>
  {
    new object[]
    {
      new FooBaz(new List<KeyValuePair<string, IEnumerable<string>>> { new("user-agent", new List<string> { "foo" }) }),
      @"{""headers"":{""user-agent"":""foo""}}",
    },
    new object[]
    {
      new FooBaz(new List<KeyValuePair<string, IEnumerable<string>>> { new("user-agent", new List<string> { "foo", "bar" }) }),
      @"{""headers"":{""user-agent"":[""foo"",""bar""]}}",
    },
    new object[]
    {
      new FooBaz(new List<KeyValuePair<string, IEnumerable<string>>> { new("user-agent", new List<string> { "foo", null! }) }),
      @"{""headers"":{""user-agent"":[""foo"",null]}}",
    },
    new object[]
    {
      new FooBaz(new List<KeyValuePair<string, IEnumerable<string>>>()),
      @"{""headers"":{}}",
    }
  };
  public static IEnumerable<object[]> Objects => new List<object[]>
  {
    new object[] { new { foo = "bar" }, @"{""foo"":""bar""}" },
    new object[] { new { Foo = "bar" }, @"{""foo"":""bar""}" },
    new object[] { new FooBar("bar"), @"{""foo"":""bar""}" }
  };

  public static IEnumerable<object[]> Strings => new List<object[]>
  {
    new object[] { @"{""foo"":""bar""}" },
    new object[] { @"{""Foo"":""bar""}" },
    new object[] { @"{""FoO"":""bar""}" }
  };

  public static IEnumerable<object[]> EnumValues => new List<object[]>
  {
    new object[] { FooEnum.Bar, @"""bar""" },
    new object[] { FooEnum.FooBar, @"""foo_bar""" },
    new object[] { FooEnum.BazQux, @"""baz-qux""" }
  };

  [Theory]
  [MemberData(nameof(Objects))]
  public void Serialize_GivenInput_ReturnsString(object input, string expected)
  {
    // act
    var result = MessageSerializer.Serialize(input);

    // assert
    result.Should().Be(expected);
  }

  [Theory]
  [MemberData(nameof(Strings))]
  public void Deserialize_GivenType_ReturnsObject(string input)
  {
    // act
    var result = MessageSerializer.Deserialize(input, typeof(FooBar));

    // assert
    result.Should().BeOfType<FooBar>();
    result.Should().BeEquivalentTo(new
    {
      Foo = "bar"
    });
  }

  [Theory]
  [MemberData(nameof(Strings))]
  public void Deserialize_GenericReturnType_ReturnsObject(string input)
  {
    // act
    var result = MessageSerializer.Deserialize<FooBar>(input);

    // assert
    result.Should().BeOfType<FooBar>();
    result.Should().BeEquivalentTo(new
    {
      Foo = "bar"
    });
  }

  [Theory]
  [MemberData(nameof(EnumValues))]
  [MemberData(nameof(HttpMethodEnumerable))]
  public void Serialize_GivenEnumValue_ReturnString(object input, string expected)
  {
    // act
    var data = MessageSerializer.Serialize(input);

    // assert
    data.Should().Be(expected);
  }

  [Theory]
  [MemberData(nameof(Headers))]
  public void Serialize_GivenKeyValuePairs_ReturnsSerializedHeaders(object input, string expected)
  {
    // act
    var result = MessageSerializer.Serialize(input);

    // assert
    result.Should().Be(expected);
  }

  [Theory]
  [MemberData(nameof(EnumValues))]
  [MemberData(nameof(HttpMethodEnumerable))]
  public void Deserialize_GivenString_ReturnEnumValue(object expected, string input)
  {
    // act
    var result = MessageSerializer.Deserialize(input, expected.GetType());

    // assert
    result.Should().Be(expected);
  }

  [Theory]
  [MemberData(nameof(Headers))]
  public void Deserialize_GivenSerializedHeaders_ReturnsKeyValuePairs(object expected, string input)
  {
    // act
    var result = MessageSerializer.Deserialize(input, expected.GetType());

    // assert
    result.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Deserialize_GivenMissingFieldInput_ReturnObject()
  {
    // act
    var result = MessageSerializer.Deserialize<Tuple<FooEnum?>>("{}");

    // assert
    result.Should().BeOfType<Tuple<FooEnum?>>();
    result!.Item1.Should().BeNull();
  }

  [Fact]
  public void Deserialize_GivenNullFieldInput_ReturnObject()
  {
    // act
    var result = MessageSerializer.Deserialize<Tuple<FooEnum?>>(@"{""item1"":null}");

    // assert
    result.Should().BeOfType<Tuple<FooEnum?>>();
    result!.Item1.Should().BeNull();
  }

  [Fact]
  public void Deserialize_GivenMissingMember_ThrowError()
  {
    // act
    var act = () => MessageSerializer.Deserialize(@"""FOO""", typeof(HttpMethod));

    // assert
    act.Should().Throw<JsonException>().WithMessage("*FOO*");
  }

  [Fact]
  public void Serialize_GivenMissingMember_ThrowError()
  {
    // act
    var act = () => MessageSerializer.Serialize(new HttpMethod("FOO"));

    // assert
    act.Should().Throw<JsonException>().WithMessage("*FOO*");
  }
}
