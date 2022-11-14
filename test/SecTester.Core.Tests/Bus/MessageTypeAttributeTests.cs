namespace SecTester.Core.Tests.Bus;

public class MessageTypeAttributeTests
{
  [MessageType(name: "custom_name")]
  private record ConcreteEvent : Event;

  [Fact]
  public void EventNameAttribute_SetsCustomName()
  {
    // arrange
    var info = typeof(ConcreteEvent);

    // act
    var attribute = info.GetCustomAttributes(typeof(MessageTypeAttribute), true).FirstOrDefault();

    // assert
    attribute.Should().BeEquivalentTo(new
    {
      Name = "custom_name"
    });
  }
}
