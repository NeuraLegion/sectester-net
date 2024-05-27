using System.Runtime.Serialization;
using MessagePack;
using SecTester.Repeater.Bus.Formatters;

namespace SecTester.Repeater.Tests.Bus.Formatters;

public sealed class MessagePackStringEnumFormatterTests
{
  public enum Foo
  {
    [EnumMember(Value = "bar")]
    Bar = 0,
    [EnumMember(Value = "baz_cux")]
    BazCux = 1
  }

  [MessagePackObject]
  public record EnumDto
  {
    [Key("foo")]
    [MessagePackFormatter(typeof(MessagePackStringEnumFormatter<Foo>))]
    public Enum Foo { get; set; }
  }

  private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard;

  public static IEnumerable<object[]>
    Fixtures =>
    new List<object[]>
    {
      new object[] { "{\"foo\":\"bar\"}" },
      new object[] { "{\"foo\":\"baz_cux\"}" }
    };

  public static IEnumerable<object[]>
    WrongValueFixtures =>
    new List<object[]>
    {
      new object[] { "{\"foo\": null}" },
      new object[] { "{\"foo\": 5}" },
      new object[] { "{\"foo\":\"BazCux\"}" }
    };


  [Theory]
  [MemberData(nameof(Fixtures))]
  public void MessagePackStringEnumFormatter_Serialize_ShouldCorrectlySerialize(
    string input)
  {
     // arrange
       var binary = MessagePackSerializer.ConvertFromJson(input, Options);
       var obj = MessagePackSerializer.Deserialize<EnumDto>(binary, Options);


       // act
       var result = MessagePackSerializer.SerializeToJson(obj, Options);

       // assert
       result.Should().BeEquivalentTo(input);
  }

  [Theory]
  [MemberData(nameof(WrongValueFixtures))]
  public void MessagePackStringEnumFormatter_Deserialize_ShouldThrowWhenDataHasWrongValue(string input)
  {
    // arrange
    var binary = MessagePackSerializer.ConvertFromJson(input, Options);

    // act
    var act = () => MessagePackSerializer.Deserialize<EnumDto>(binary, Options);

    // assert
    act.Should().Throw<MessagePackSerializationException>().WithMessage(
      "Failed to deserialize*");
  }
}
