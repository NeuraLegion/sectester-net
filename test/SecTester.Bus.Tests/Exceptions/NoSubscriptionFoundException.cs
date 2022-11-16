namespace SecTester.Bus.Tests.Exceptions;

public class NoSubscriptionFoundExceptionTests
{
  [Fact]
  public void NoSubscriptionFoundException_SingleEventName_CreatesInstance()
  {
    // arrange
    const string eventName = "event1";

    // act
    var result = new NoSubscriptionFoundException(eventName);

    // assert
    result.Should()
      .Match<NoSubscriptionFoundException>(x =>
        x.Message.EndsWith($"Please register a handler for the {eventName} event in the event bus."));
  }
}
