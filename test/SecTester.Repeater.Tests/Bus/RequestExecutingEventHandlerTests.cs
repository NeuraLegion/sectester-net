namespace SecTester.Repeater.Tests.Bus;

public class RequestExecutingEventHandlerTests : IDisposable
{
  private readonly RequestRunnerResolver _resolver = Substitute.For<RequestRunnerResolver>();
  private readonly RequestExecutingEventListener _sut;

  public RequestExecutingEventHandlerTests()
  {
    _sut = new RequestExecutingEventListener(_resolver);
  }

  public void Dispose()
  {
    _resolver.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task Handle_RunRequestHavingCorrespondingRunner()
  {
    // arrange
    var @event = new RequestExecutingEvent(new Uri("http://foo.bar"))
    {
      Protocol = Protocol.Http
    };
    var reply = new RequestExecutingResult
    {
      Protocol = Protocol.Http,
      StatusCode = 200,
      Body = "text"
    };
    _resolver(Protocol.Http)!.Run(@event).Returns(reply);

    // act
    var result = await _sut.Handle(@event);

    // assert
    result.Should().BeEquivalentTo(reply);
  }

  [Fact]
  public async Task Handle_NoRunnerFound_ThrowsError()
  {
    // arrange
    var @event = new RequestExecutingEvent(new Uri("http://foo.bar"))
    {
      Protocol = Protocol.Http
    };
    _resolver(Arg.Any<Protocol>()).ReturnsNull();

    // act
    var act = () => _sut.Handle(@event);

    // assert
    await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Unsupported protocol {Protocol.Http}");
  }
}
