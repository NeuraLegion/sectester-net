using System.Runtime.Serialization;
using MessagePack;
using MessagePack.Resolvers;
using SecTester.Repeater.Internal;

namespace SecTester.Repeater.Tests.Internal;

public enum Foo
{
  Foo = 0,
  BAR = 1,
  [EnumMember(Value = "baz_cux")]
  BazCux = 2
}

public sealed class MessagePackStringEnumMemberFormatterTests
{
  private static readonly MessagePackSerializerOptions Options = new(
    CompositeResolver.Create(
      CompositeResolver.Create(
        new MessagePackStringEnumMemberFormatter<Foo>(MessagePackNamingPolicy.SnakeCase)
        ),
      BuiltinResolver.Instance
    )
  );

  public static IEnumerable<object[]>
    Fixtures =>
    new List<object[]>
    {
      new object[] { "foo", Foo.Foo },
      new object[] { "bar", Foo.BAR },
      new object[] { "baz_cux", Foo.BazCux }
    };

  public static IEnumerable<object[]>
    WrongValueFixtures =>
    new List<object[]>
    {
      new object[] { null },
      new object[] { "5" },
      new object[] { "BazCux" }
    };

  [Theory]
  [MemberData(nameof(Fixtures))]
  public void MessagePackStringEnumFormatter_Serialize_ShouldCorrectlySerialize(
    string input, Foo expected)
  {
    // arrange
    var binary = MessagePackSerializer.Serialize<string>(input, Options);


    // act
    var result = MessagePackSerializer.Deserialize<Foo>(binary, Options);

    // assert
    result.Should().Be(expected);
  }

  [Theory]
  [MemberData(nameof(WrongValueFixtures))]
  public void MessagePackStringEnumFormatter_Deserialize_ShouldThrowWhenDataHasWrongValue(string input)
  {
    // arrange
    var binary = MessagePackSerializer.Serialize<string>(input, Options);

    // act
    var act = () => MessagePackSerializer.Deserialize<Foo>(binary, Options);

    // assert
    act.Should().Throw<MessagePackSerializationException>().WithMessage(
      "Failed to deserialize*");
  }
}
