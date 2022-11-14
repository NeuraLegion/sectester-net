namespace SecTester.Core.Tests.Utils;

public class MessageUtilsTests
{
  public static readonly object[][] Types =
  {
    new object[]
    {
      typeof(ConcreteEvent), nameof(ConcreteEvent)
    },
    new object[]
    {
      typeof(ConcreteEvent2), nameof(ConcreteEvent)
    },
    new object[]
    {
      typeof(ConcreteEvent3), "custom"
    }
  };

  [Theory]
  [MemberData(nameof(Types))]
  public void MessageUtils_GivenType_ReturnsType(Type input, string expected)
  {
    // act
    var result = MessageUtils.GetMessageType(input);

    // assert
    result.Should().Be(expected);
  }

  private record ConcreteEvent : Event;

  [MessageType(name: nameof(ConcreteEvent))]
  private record ConcreteEvent2 : Event;

  [MessageType(name: "custom")]
  private record ConcreteEvent3 : Event;
}
