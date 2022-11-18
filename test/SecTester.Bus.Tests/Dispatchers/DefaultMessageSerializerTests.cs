using SecTester.Bus.Tests.Fixtures;

namespace SecTester.Bus.Tests.Dispatchers;

public class DefaultMessageSerializerTests
{
  private readonly DefaultMessageSerializer _sut;

  public DefaultMessageSerializerTests()
  {
    _sut = new DefaultMessageSerializer();
  }

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
    new object[] { typeof(FooEnum), FooEnum.Bar, @"""bar""" },
    new object[] { typeof(FooEnum), FooEnum.FooBar, @"""foo_bar""" },
    new object[] { typeof(FooEnum), FooEnum.BazQux, @"""baz-qux""" },
    new object[] { typeof(FooEnum?), null!, @"null" },
  };

  [Theory]
  [MemberData(nameof(Objects))]
  public void Serialize_GivenInput_ReturnsString(object input, string expected)
  {
    // act
    var result = _sut.Serialize(input);

    // assert
    result.Should().Be(expected);
  }

  [Theory]
  [MemberData(nameof(Strings))]
  public void Deserialize_GivenType_ReturnsObject(string input)
  {
    // act
    var result = _sut.Deserialize(input, typeof(FooBar));

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
    var result = _sut.Deserialize<FooBar>(input);

    // assert
    result.Should().BeOfType<FooBar>();
    result.Should().BeEquivalentTo(new
    {
      Foo = "bar"
    });
  }
  
  [Theory]
  [MemberData(nameof(EnumValues))]
  public void Serialize_GivenEnumValue_ReturnString(Type type, object value, string valueString)
  {
    // act
    var data = _sut.Serialize(value);

    // assert
    data.Should().Be(valueString);
  }

  [Theory]
  [MemberData(nameof(EnumValues))]
  public void  Deserialize_GivenString_ReturnEnumValue(Type type, object value, string valueString)
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
    var result = _sut.Deserialize<Tuple<FooEnum?>>("{}");

    // assert
    result.Should().BeOfType<Tuple<FooEnum?>>();
    result!.Item1.Should().BeNull();
  }
}
