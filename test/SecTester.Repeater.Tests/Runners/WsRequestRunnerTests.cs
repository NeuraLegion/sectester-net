using SecTester.Repeater.Tests.Fixtures;

namespace SecTester.Repeater.Tests.Runners;

public class WsRequestRunnerTests : IClassFixture<TestServerApplicationFixture<Startup>>, IAsyncDisposable
{
  private readonly TestServerApplicationFixture<Startup> _fixture;
  private readonly WebSocket _mockWsClient = Substitute.For<WebSocket>();
  private readonly WsClientFactory _mockWsClientFactory = Substitute.For<WsClientFactory>();

  public WsRequestRunnerTests(TestServerApplicationFixture<Startup> fixture)
  {
    _fixture = fixture;
  }

  public async ValueTask DisposeAsync()
  {
    _mockWsClient.ClearSubstitute();
    _mockWsClientFactory.ClearSubstitute();
    await _fixture.DisposeAsync();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task Run_ReturnsResult_WhenRequestIsSuccessful()
  {
    // arrange
    const string body = "foo";
    var request = new RequestExecutingEvent(_fixture.Url)
    {
      Body = body
    };
    var sut = _fixture.CreateWsRequestRunner();

    // act
    var result = await sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Protocol = Protocol.Ws,
      StatusCode = 1000,
      Body = body
    });
  }

  [Fact]
  public async Task Run_ReturnsResult_WhenResponseIsChunked()
  {
    // arrange
    const string body = "chunked";
    var request = new RequestExecutingEvent(_fixture.Url)
    {
      Body = body
    };
    var sut = _fixture.CreateWsRequestRunner();

    // act
    var result = await sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Protocol = Protocol.Ws,
      StatusCode = 1000,
      Body = "ping pong"
    });
  }

  [Fact]
  public async Task Run_ReturnsResultInCorrectOrder()
  {
    // arrange
    var range = Enumerable.Range(0, 5);
    var bodies = range.Select(idx => $"echo:{idx}");
    var requests = bodies.Select(body => new RequestExecutingEvent(_fixture.Url)
    {
      Body = body
    });
    var sut = _fixture.CreateWsRequestRunner();

    // act
    var result = await Task.WhenAll(requests.Select(request => sut.Run(request)));

    // assert
    result.Should().BeInAscendingOrder(x => x.Body);
  }

  [Fact]
  public async Task Run_ReturnsResultMatchedWithCorrelationIdRegex()
  {
    // arrange
    const string body = "range";
    const string expected = "range:2";
    var request = new RequestExecutingEvent(_fixture.Url)
    {
      Body = body,
      CorrelationIdRegex = new Regex(@":2$")
    };
    var sut = _fixture.CreateWsRequestRunner();

    // act
    var result = await sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Body = expected
    });
  }

  [Fact]
  public async Task Run_WebSocketClosed_ReturnsResponseWithStatusCode()
  {
    // arrange
    const string body = "close-output";
    var request = new RequestExecutingEvent(_fixture.Url)
    {
      Body = body
    };
    var sut = _fixture.CreateWsRequestRunner();

    // act
    var result = await sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Protocol = Protocol.Ws,
      StatusCode = 1007,
      Message = "invalid payload"
    });
  }

  [Fact]
  public async Task Run_WebSocketException_ReturnsResponseWithErrorCode()
  {
    // arrange
    const string body = "foo";
    var request = new RequestExecutingEvent(_fixture.Url)
    {
      Body = body
    };
    var sut = _fixture.CreateWsRequestRunner(_mockWsClientFactory);
    _mockWsClientFactory.CreateWsClient(Arg.Any<Uri>(), Arg.Any<CancellationToken>()).Returns(_mockWsClient);
    _mockWsClient.SendAsync(Arg.Any<ArraySegment<byte>>(), Arg.Any<WebSocketMessageType>(), true, Arg.Any<CancellationToken>())
      .ThrowsAsync(new WebSocketException(WebSocketError.UnsupportedProtocol));

    // act
    var result = await sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Protocol = Protocol.Ws,
      ErrorCode = "UnsupportedProtocol",
      Message = "The WebSocket request or response operation was called with unsupported protocol(s)."
    });
  }

  [Fact]
  public async Task Run_ReturnsResultWithError_WhenRequestTimesOut()
  {
    // arrange
    const string body = "echo";
    var request = new RequestExecutingEvent(_fixture.Url)
    {
      Body = body
    };
    var sut = _fixture.CreateWsRequestRunner(new RequestRunnerOptions
    {
      Timeout = TimeSpan.FromMilliseconds(50)
    });

    // act
    var result = await sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Protocol = Protocol.Ws,
      Message = "The operation was canceled."
    });
  }
}





