using SecTester.Bus.Tests.Fixtures;

namespace SecTester.Bus.Tests.Dispatchers;

public class HttpCommandDispatcherTests : IDisposable
{
  private const string BaseUrl = "https://example.com";
  private const string Token = "0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk";
  private readonly MockHttpMessageHandler _mockHttp;
  private readonly HttpCommandDispatcher _dispatcher;
  private readonly MessageSerializer _messageSerializer;
  private readonly IHttpClientFactory _httpClientFactory;

  public HttpCommandDispatcherTests()
  {
    _mockHttp = new MockHttpMessageHandler();
    _httpClientFactory = Substitute.For<IHttpClientFactory>();
    _messageSerializer = Substitute.For<DefaultMessageSerializer>();
    var config = new HttpCommandDispatcherConfig(BaseUrl, Token);
    _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(_mockHttp.ToHttpClient());
    _dispatcher = new HttpCommandDispatcher(_httpClientFactory, config, _messageSerializer);
  }

  public void Dispose()
  {
    _httpClientFactory.ClearSubstitute();
    _messageSerializer.ClearSubstitute();
    _mockHttp.Clear();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task Execute_GivenCommand_SendsWithAuthorizationHeader()
  {
    // arrange
    const string path = "/api/test";
    var command = new HttpRequest<Unit>(path, expectReply: false);

    _mockHttp.Expect($"{BaseUrl}{path}")
      .WithHeaders("authorization", $"api-key {Token}")
      .Respond(HttpStatusCode.NoContent);

    // act
    await _dispatcher.Execute(command);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
  }

  [Fact]
  public async Task Execute_GivenCommand_SendsCorrelationIdHeader()
  {
    // arrange
    const string path = "/api/test";
    var command = new HttpRequest<Unit>(path, expectReply: false);

    _mockHttp.Expect($"{BaseUrl}{path}")
      .WithHeaders("x-correlation-id", command.CorrelationId)
      .Respond(HttpStatusCode.NoContent);

    // act
    await _dispatcher.Execute(command);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
  }

  [Fact]
  public async Task Execute_GivenCommand_SendsDateHeader()
  {
    // arrange
    const string path = "/api/test";
    var command = new HttpRequest<Unit>(path, expectReply: false);

    _mockHttp.Expect($"{BaseUrl}{path}")
      .WithHeaders("date", command.CreatedAt.ToUniversalTime().ToString("r"))
      .Respond(HttpStatusCode.NoContent);

    // act
    await _dispatcher.Execute(command);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
  }

  [Fact]
  public async Task Execute_PostMethodAndBodyExists_SendsBody()
  {
    // arrange
    const string path = "/api/test";
    const string body = @"{""foo"":""bar""}";
    var command = new HttpRequest<Unit>(path, HttpMethod.Post, body: body, expectReply: false);

    _mockHttp.Expect(HttpMethod.Post, $"{BaseUrl}{path}")
      .WithContent(body)
      .Respond(HttpStatusCode.NoContent);

    // act
    await _dispatcher.Execute(command);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
  }

  [Fact]
  public async Task Execute_GivenCommand_ReturnsReplay()
  {
    // arrange
    const string path = "/api/test";
    var command = new HttpRequest<BazQux>(path);

    _mockHttp.Expect($"{BaseUrl}{path}")
      .Respond("application/json", @"{""baz"":""qux""}");

    // act
    var result = await _dispatcher.Execute(command);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
    result.Should().BeEquivalentTo(new { Baz = "qux" });
  }

  [Fact]
  public async Task Execute_ExpectReplyIsFalse_ReturnsNullImmediately()
  {
    // arrange
    const string path = "/api/test";
    var command = new HttpRequest<BazQux>(path, expectReply: false);

    _mockHttp.Expect($"{BaseUrl}{path}")
      .Respond("application/json", @"{""baz"":""qux""}");

    // act
    var result = await _dispatcher.Execute(command);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
    result.Should().BeNull();
  }

  [Fact]
  public async Task Execute_NoResponse_ThrowError()
  {
    // arrange
    const string path = "/api/test";
    var command = new HttpRequest<BazQux>(path, ttl: 1);

    _mockHttp.Expect($"{BaseUrl}{path}")
      .Respond(async () =>
      {
        await Task.Delay(5);

        return new HttpResponseMessage(HttpStatusCode.OK);
      });

    // act
    var act = () => _dispatcher.Execute(command);

    // assert
    await act.Should().ThrowAsync<Exception>();
    _mockHttp.VerifyNoOutstandingExpectation();
  }

  [Fact]
  public async Task Execute_NonSuccessStatusCode_ThrowError()
  {
    // arrange
    const string path = "/api/test";
    var command = new HttpRequest<BazQux>(path);

    _mockHttp.Expect($"{BaseUrl}{path}")
      .Respond(HttpStatusCode.BadGateway);

    // act
    var act = () => _dispatcher.Execute(command);

    // assert
    await act.Should().ThrowAsync<Exception>();
    _mockHttp.VerifyNoOutstandingExpectation();
  }
}
