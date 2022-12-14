using SecTester.Core.Tests.Fixtures;

namespace SecTester.Core.Tests.Bus;

public class EventTests : IDisposable
{
  private readonly IEventDispatcher _dispatcher;

  public EventTests()
  {
    _dispatcher = Substitute.For<IEventDispatcher>();
  }

  public void Dispose()
  {
    _dispatcher.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

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
}
