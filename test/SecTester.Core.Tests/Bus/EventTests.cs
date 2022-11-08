using SecTester.Core.Bus;

namespace SecTester.Core.Tests.Bus;

public class EventTests : IDisposable
{
  private readonly EventDispatcher _dispatcher;

  public EventTests()
  {
    _dispatcher = Substitute.For<EventDispatcher>();
  }

  public void Dispose() => _dispatcher.ClearSubstitute();

  [Fact]
  public void Event_Publishes()
  {
    // arrange
    const string payload = "text";
    var @event = new TestEvent(Payload: payload);
    _dispatcher.Publish(@event).Returns(Task.CompletedTask);

    // act
    @event.Publish(_dispatcher);

    // assert
    _dispatcher.Received(1).Publish(@event);
  }

  [Fact]
  public void Command_WhenException_RethrowsError()
  {
    // arrange
    const string payload = "text";
    var @event = new TestEvent(Payload: payload);
    _dispatcher.Publish(@event).ThrowsAsync<Exception>();

    // act
    var act = () => @event.Publish(_dispatcher);

    // assert
    act.Should().ThrowAsync<Exception>();
  }

  private record TestEvent(string Payload) : Event
  {
    public string Payload = Payload;
  }
}
