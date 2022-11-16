using SecTester.Core.Tests.Fixtures;

namespace SecTester.Core.Tests.Bus;

public class MessageTypeAttributeTests
{
  [Fact]
  public void EventNameAttribute_SetsCustomName()
  {
    // arrange
    var info = typeof(TestEvent2);

    // act
    var attribute = info.GetCustomAttributes(typeof(MessageTypeAttribute), true).FirstOrDefault();

    // assert
    attribute.Should().BeEquivalentTo(new
    {
      Name = "custom"
    });
  }
}
