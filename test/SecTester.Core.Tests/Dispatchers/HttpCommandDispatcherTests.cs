using System.Net;
using System.Net.Http.Json;

using SecTester.Core.Tests.Fixtures;

namespace SecTester.Core.Tests.Dispatchers;

public class HttpCommandDispatcherTests : IDisposable
{
  private const string BaseUrl = "https://example.com";
  private const string Token = "0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk";
  private readonly HttpCommandDispatcher _dispatcher;
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly MockHttpMessageHandler _mockHttp;
  private readonly IRetryStrategy _retryStrategy;

  public HttpCommandDispatcherTests()
  {
    _mockHttp = new MockHttpMessageHandler();
    _httpClientFactory = Substitute.For<IHttpClientFactory>();
    _retryStrategy = Substitute.For<IRetryStrategy>();
    _retryStrategy.Acquire(Arg.Any<Func<Task<HttpResponseMessage>>>(), Arg.Any<CancellationToken>())
      .Returns(x => x.ArgAt<Func<Task<HttpResponseMessage>>>(0).Invoke());
    _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(_mockHttp.ToHttpClient());

    var config = new HttpCommandDispatcherConfig(BaseUrl, Token);
    _dispatcher = new HttpCommandDispatcher(_httpClientFactory, config, _retryStrategy);
  }

  public void Dispose()
  {
    _retryStrategy.ClearSubstitute();
    _httpClientFactory.ClearSubstitute();
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
  public async Task Execute_QueryParameters_SendsWithQueryString()
  {
    // arrange
    const string path = "/api/test";
    var command = new HttpRequest<Unit>(path, expectReply: false, @params: new Dictionary<string, string>
    {
      {
        "foo", "bar"
      }
    });

    _mockHttp.Expect($"{BaseUrl}{path}")
      .WithQueryString("foo=bar")
      .WithHeaders("authorization", $"api-key {Token}")
      .Respond(HttpStatusCode.NoContent);

    // act
    await _dispatcher.Execute(command);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
  }

  [Fact]
  public async Task Execute_QueryParameters_AppendsQueryToExistingInUri()
  {
    // arrange
    const string path = "/api/test?foo=bar";
    var command = new HttpRequest<Unit>(path, expectReply: false, @params: new Dictionary<string, string>
    {
      {
        "baz", "xyzzy"
      }
    });

    _mockHttp.Expect($"{BaseUrl}{path}")
      .WithQueryString("foo=bar&baz=xyzzy")
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
    var body = JsonContent.Create(new FooBar("bar"), typeof(FooBar));
    var command = new HttpRequest<Unit>(path, HttpMethod.Post, body: body, expectReply: false);
    var expectedBody = await body.ReadAsStringAsync().ConfigureAwait(false);

    _mockHttp.Expect(HttpMethod.Post, $"{BaseUrl}{path}")
      .WithContent(expectedBody)
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
    result.Should().BeEquivalentTo(new
    {
      Baz = "qux"
    });
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
    var command = new HttpRequest<BazQux>(path, ttl: TimeSpan.FromMilliseconds(1));

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

  [Fact]
  public async Task Execute_NonSuccessStatusCode_RetriesRequest()
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
    await _retryStrategy.Received().Acquire(Arg.Any<Func<Task<HttpResponseMessage>>>(), Arg.Any<CancellationToken>());
    _mockHttp.VerifyNoOutstandingExpectation();
  }
}
