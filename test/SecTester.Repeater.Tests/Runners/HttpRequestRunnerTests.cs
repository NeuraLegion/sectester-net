namespace SecTester.Repeater.Tests.Runners;

public class HttpRequestRunnerTests : IDisposable
{
  private const string Url = "https://example.com";
  private const string JsonContentType = "application/json";
  private const string CustomContentType = "application/x-custom";
  private const string Content = @"{""foo"":""bar""}";
  private const string HeaderFieldValue = "test-header-value";
  private const string HeaderFieldName = "testHeader";
  private const string ContentLengthFieldName = "Content-Length";
  private const string ContentTypeFieldName = "Content-Type";

  private readonly IHttpClientFactory _httpClientFactory = Substitute.For<IHttpClientFactory>();
  private readonly MockHttpMessageHandler _mockHttp = new();

  private readonly RequestRunnerOptions _options = new()
  {
    MaxContentLength = 1
  };

  private readonly HttpRequestRunner _sut;

  public HttpRequestRunnerTests()
  {
    _sut = new HttpRequestRunner(_options, _httpClientFactory);
    _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(_mockHttp.ToHttpClient());
  }

  public void Dispose()
  {
    _httpClientFactory.ClearSubstitute();
    _mockHttp.Clear();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task Run_PerformAnHttpRequest()
  {
    // arrange
    var headers = new[]
    {
      new KeyValuePair<string, IEnumerable<string>>(HeaderFieldName, new[]
      {
        HeaderFieldValue
      })
    };
    var request = new RequestExecutingEvent(new Uri(Url))
    {
      Headers = headers
    };
    _mockHttp.Expect(Url).WithHeaders(headers.Select(x => new KeyValuePair<string, string>(x.Key, string.Join(";", x.Value))))
      .Respond(HttpStatusCode.OK, JsonContentType, Content);

    // act
    var result = await _sut.Run(request);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
    result.Should().BeEquivalentTo(new
    {
      StatusCode = (int)HttpStatusCode.OK,
      Body = Content
    });
  }

  [Fact]
  public async Task Run_NoContentStatusReceived_SkipsTruncating()
  {
    // arrange
    var request = new RequestExecutingEvent(new Uri(Url));
    _mockHttp.Expect(Url).Respond(HttpStatusCode.NoContent);

    // act
    var result = await _sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      StatusCode = (int)HttpStatusCode.NoContent
    });
  }

  [Fact]
  public async Task Run_HeadMethodUsed_SkipsTruncating()
  {
    // arrange
    var request = new RequestExecutingEvent(new Uri(Url))
    {
      Method = HttpMethod.Head
    };
    _mockHttp.Expect(Url).Respond(HttpStatusCode.OK, JsonContentType, Content);

    // act
    var result = await _sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      StatusCode = (int)HttpStatusCode.OK,
      Body = ""
    });
  }

  [Fact]
  public async Task Run_AllowedMimeReceived_SkipsTruncating()
  {
    // arrange
    var request = new RequestExecutingEvent(new Uri(Url));
    _mockHttp.Expect(Url).Respond(HttpStatusCode.OK, JsonContentType, Content);

    // act
    var result = await _sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      StatusCode = (int)HttpStatusCode.OK,
      Body = Content
    });
  }

  [Fact]
  public async Task Run_NotAllowedMimeReceived_TruncatesBody()
  {
    // arrange
    var headers = new[]
    {
      new KeyValuePair<string, IEnumerable<string>>(ContentTypeFieldName, new[]
      {
        $"{CustomContentType}; charset=utf-8"
      }),
      new KeyValuePair<string, IEnumerable<string>>(ContentLengthFieldName, new[]
      {
        $"{_options.MaxContentLength}"
      })
    };
    var request = new RequestExecutingEvent(new Uri(Url));
    var body = string.Concat(Enumerable.Repeat("x", 5));
    _mockHttp.Expect(Url).Respond(HttpStatusCode.OK, CustomContentType, body);

    // act
    var result = await _sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Headers = headers,
      StatusCode = (int)HttpStatusCode.OK,
      Body = "x"
    }, x => x.ExcludingMissingMembers());
  }

  [Fact]
  public async Task Run_HttpStatusException_ReturnsResponse()
  {
    // arrange
    var request = new RequestExecutingEvent(new Uri(Url));
    _mockHttp.Expect(Url).Respond(HttpStatusCode.ServiceUnavailable, JsonContentType, Content);

    // act
    var result = await _sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      StatusCode = (int)HttpStatusCode.ServiceUnavailable,
      Body = Content
    });
  }

  [Fact]
  public async Task Run_TcpException_ReturnsResponse()
  {
    // arrange
    var request = new RequestExecutingEvent(new Uri(Url));
    _mockHttp.Expect(Url).Throw(new SocketException((int)SocketError.ConnectionRefused));

    // act
    var result = await _sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      ErrorCode = "ConnectionRefused"
    }, options => options.Using<Response>(ctx => ctx.Subject.Should().BeOfType<string>()).When(info => info.Path.EndsWith("Message")));
  }
}
