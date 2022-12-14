using SecTester.Bus.Tests.Fixtures;

namespace SecTester.Bus.Tests.Extensions;

public class GenericTypeExtensionsTests
{
  [Fact]
  public void GetConcreteEventListenerType_ReturnTypeSpecified_ReturnsConcreteType()
  {
    // arrange
    var type = typeof(ConcreteFirstHandler);

    // act
    var result = type.GetConcreteEventListenerType();

    // assert
    result.Should().Be(typeof(IEventListener<ConcreteEvent, FooBar>));
  }

  [Fact]
  public void GetConcreteEventListenerType_ReturnTypeIsVoid_ReturnsConcreteType()
  {
    // arrange
    var type = typeof(ConcreteSecondHandler);

    // act
    var result = type.GetConcreteEventListenerType();

    // assert
    result.Should().Be(typeof(IEventListener<ConcreteEvent, Unit>));
  }
}
