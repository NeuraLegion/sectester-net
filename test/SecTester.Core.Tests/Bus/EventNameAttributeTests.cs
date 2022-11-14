using SecTester.Core.Bus;

namespace SecTester.Core.Tests.Bus;

public class EventNameAttributeTests
{
  [EventName(name: "custom_name")]
  private record ConcreteEvent(string Payload) : Event
  {
    public string Payload = Payload;
  }

  [Fact]
  public void EventNameAttribute_SetsCustomName()
  {
    // arrange
    var info = typeof(ConcreteEvent);

    // act
    var attribute = info.GetCustomAttributes(typeof(EventNameAttribute), true).FirstOrDefault();

    // assert
    attribute.Should().BeEquivalentTo(new
    {
      Name = "custom_name"
    });
  }
}
