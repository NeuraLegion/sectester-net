namespace SecTester.Bus.Tests.Exceptions;

public class EventHandlerNotFoundExceptionTests
{
  private const string DefaultPostfix = "Please register a handler for the following events";

  [Fact]
  public void EventHandlerNotFoundException_SingleEventName_CreatesInstance()
  {
    // arrange
    const string eventName = "event1";

    // act
    var result = new EventHandlerNotFoundException(eventName);

    // assert
    result.Should()
      .Match<EventHandlerNotFoundException>(x => x.Message.EndsWith($"{DefaultPostfix}: {eventName}"));
  }

  [Fact]
  public void EventHandlerNotFoundException_MultipleEventNames_CreatesInstance()
  {
    // arrange
    const string eventName = "event1";
    const string eventName2 = "event2";

    // act
    var result = new EventHandlerNotFoundException(eventName, eventName2);

    // assert
    result.Should()
      .Match<EventHandlerNotFoundException>(x =>
        x.Message.EndsWith($"{DefaultPostfix}: {string.Join(", ", eventName, eventName2)}"));
  }
}
