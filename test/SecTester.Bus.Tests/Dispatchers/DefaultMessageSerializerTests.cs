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
}
