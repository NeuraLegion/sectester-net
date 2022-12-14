namespace SecTester.Repeater.Tests.Runners;

public class HttpRequestRunnerTests : IDisposable
{
  private const string Url = "https://example.com";
  private const string JsonContentType = "application/json";
  private const string HtmlContentType = "text/html";
  private const string HtmlContentTypeWithCharSet = $"{HtmlContentType}; charset=utf-16";
  private const string CustomContentType = "application/x-custom";
  private const string CustomContentTypeWithUtf8CharSet = $"{CustomContentType}; charset=utf-8";
  private const string JsonContent = @"{""foo"":""bar""}";
  private const string HtmlBody = "<html></html>";
  private const string HeaderFieldValue = "test-header-value";
  private const string HeaderFieldName = "X-Test-Header";
  private const string ContentLengthFieldName = "Content-Length";
  private const string ContentTypeFieldName = "Content-Type";
  private const string HostFieldName = "Host";
  private const string InvalidHostHeaderValue = "\0example.com\n";
  private static readonly Uri Uri = new(Url);

  private readonly IHttpClientFactory _httpClientFactory = Substitute.For<IHttpClientFactory>();
  private readonly MockHttpMessageHandler _mockHttp = new();

  public void Dispose()
  {
    _httpClientFactory.ClearSubstitute();
    _mockHttp.Clear();
    _mockHttp.Dispose();
    GC.SuppressFinalize(this);
  }

  private HttpRequestRunner CreateSut(RequestRunnerOptions? options = default)
  {
    _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(_mockHttp.ToHttpClient());
    return new HttpRequestRunner(options ?? new RequestRunnerOptions(), _httpClientFactory);
  }

  [Fact]
  public async Task Run_ReturnsResult_WhenRequestIsSuccessful()
  {
    // arrange
    var sut = CreateSut();
    var headers = new[]
    {
      new KeyValuePair<string, IEnumerable<string>>(ContentTypeFieldName, new[]
      {
        JsonContentType
      }),
      new KeyValuePair<string, IEnumerable<string>>(HeaderFieldName, new[]
      {
        HeaderFieldValue
      })
    };
    var request = new RequestExecutingEvent(Uri)
    {
      Method = HttpMethod.Patch,
      Body = JsonContent,
      Headers = headers
    };
    _mockHttp.Expect(Url)
      .WithContent(JsonContent)
      .WithHeaders($"{HeaderFieldName}: {HeaderFieldValue}")
      .With(message => message.Method.Equals(HttpMethod.Patch))
      .With(message =>
        (bool)message.Content?.Headers.ContentType?.MediaType?.StartsWith(JsonContentType, StringComparison.OrdinalIgnoreCase))
      .Respond(HttpStatusCode.OK, JsonContentType, JsonContent);

    // act
    var result = await sut.Run(request);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
    result.Should().BeEquivalentTo(new
    {
      StatusCode = (int)HttpStatusCode.OK,
      Body = JsonContent
    });
  }

  [Fact]
  public async Task Run_ReturnsResultWithDecodedBody()
  {
    // arrange
    var sut = CreateSut();
    var encoding = Encoding.GetEncoding("utf-16");
    var expectedByteLength = Buffer.ByteLength(encoding.GetBytes(HtmlBody));
    var request = new RequestExecutingEvent(Uri);
    var content = new StringContent(HtmlBody, encoding, HtmlContentType);
    _mockHttp.Expect(Url).Respond(HttpStatusCode.OK, content);

    // act
    var result = await sut.Run(request);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
    result.Should().BeEquivalentTo(new
    {
      Headers = new[]
      {
        new KeyValuePair<string, string[]>(ContentTypeFieldName, new[]
        {
          HtmlContentTypeWithCharSet
        }),
        new KeyValuePair<string, string[]>(ContentLengthFieldName, new[]
        {
          $"{expectedByteLength}"
        })
      },
      Body = HtmlBody
    }, options => options.ExcludingMissingMembers().IncludingNestedObjects());
  }

  [Fact]
  public async Task Run_ReturnsResultWithError_WhenRequestTimesOut()
  {
    // arrange
    var sut = CreateSut(new RequestRunnerOptions
    {
      Timeout = TimeSpan.Zero
    });
    var request = new RequestExecutingEvent(Uri);
    _mockHttp.Expect(Url)
      .Respond(async () =>
      {
        await Task.Delay(5);

        return new HttpResponseMessage(HttpStatusCode.OK);
      });

    // act
    var result = await sut.Run(request);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
    result.Should().BeEquivalentTo(new
    {
      Message = "The operation was canceled."
    });
  }

  [Fact]
  public async Task Run_MaxContentLengthIsLessThan0_SkipsTruncating()
  {
    // arrange
    var sut = CreateSut(new RequestRunnerOptions
    {
      MaxContentLength = -1
    });
    var request = new RequestExecutingEvent(Uri);
    var body = string.Concat(Enumerable.Repeat("x", 5));
    _mockHttp.Expect(Url).Respond(HttpStatusCode.OK, CustomContentType, body);

    // act
    var result = await sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Body = body
    });
  }

  [Fact]
  public async Task Run_NoContentStatusReceived_SkipsTruncating()
  {
    // arrange
    var sut = CreateSut();
    var request = new RequestExecutingEvent(Uri);
    _mockHttp.Expect(Url).Respond(HttpStatusCode.NoContent);

    // act
    var result = await sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      StatusCode = (int)HttpStatusCode.NoContent,
      Body = ""
    });
  }

  [Fact]
  public async Task Run_HeadMethodUsed_SkipsTruncating()
  {
    // arrange
    var sut = CreateSut();
    var request = new RequestExecutingEvent(Uri)
    {
      Method = HttpMethod.Head
    };
    _mockHttp.Expect(Url).Respond(HttpStatusCode.OK, JsonContentType, JsonContent);

    // act
    var result = await sut.Run(request);

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
    var sut = CreateSut();
    var request = new RequestExecutingEvent(Uri);
    _mockHttp.Expect(Url).Respond(HttpStatusCode.OK, JsonContentType, JsonContent);

    // act
    var result = await sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      StatusCode = (int)HttpStatusCode.OK,
      Body = JsonContent
    });
  }

  [Fact]
  public async Task Run_NotAllowedMimeReceived_TruncatesBody()
  {
    // arrange
    var options = new RequestRunnerOptions
    {
      MaxContentLength = 1
    };
    var sut = CreateSut(options);
    var headers = new[]
    {
      new KeyValuePair<string, IEnumerable<string>>(ContentTypeFieldName, new[]
      {
        CustomContentTypeWithUtf8CharSet
      }),
      new KeyValuePair<string, IEnumerable<string>>(ContentLengthFieldName, new[]
      {
        $"{options.MaxContentLength}"
      })
    };
    var request = new RequestExecutingEvent(Uri);
    var body = string.Concat(Enumerable.Repeat("x", 5));
    _mockHttp.Expect(Url).Respond(HttpStatusCode.OK, CustomContentType, body);

    // act
    var result = await sut.Run(request);

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
    var sut = CreateSut();
    var request = new RequestExecutingEvent(Uri);
    _mockHttp.Expect(Url).Respond(HttpStatusCode.ServiceUnavailable, JsonContentType, JsonContent);

    // act
    var result = await sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      StatusCode = (int)HttpStatusCode.ServiceUnavailable,
      Body = JsonContent
    });
  }

  [Fact]
  public async Task Run_TcpException_ReturnsResponse()
  {
    // arrange
    var sut = CreateSut();
    var request = new RequestExecutingEvent(Uri);
    _mockHttp.Expect(Url).Throw(new SocketException((int)SocketError.ConnectionRefused));

    // act
    var result = await sut.Run(request);

    // assert
    result.Should().BeEquivalentTo(new
    {
      ErrorCode = "ConnectionRefused"
    },
      options => options.Using<IResponse>(ctx => ctx.Subject.Should().BeOfType<string>())
        .When(info => info.Path.EndsWith(nameof(RequestExecutingResult.Message))));
  }

  [Fact]
  public async Task Run_BypassesStrictHttpValidation()
  {
    // arrange
    var sut = CreateSut();
    var headers = new[]
    {
      new KeyValuePair<string, IEnumerable<string>>(HostFieldName, new[]
      {
        InvalidHostHeaderValue
      })
    };
    var request = new RequestExecutingEvent(Uri)
    {
      Headers = headers
    };
    _mockHttp.Expect(Url)
      .With(message => message.Headers.ToString().Contains(InvalidHostHeaderValue, StringComparison.OrdinalIgnoreCase))
      .Respond(HttpStatusCode.NoContent);

    // act
    await sut.Run(request);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
  }

  [Fact]
  public async Task Run_AcceptsContentHeaders()
  {
    // arrange
    var sut = CreateSut();
    var headers = new[]
    {
      new KeyValuePair<string, IEnumerable<string>>(ContentTypeFieldName, new[]
      {
        JsonContentType
      })
    };
    var request = new RequestExecutingEvent(Uri)
    {
      Method = HttpMethod.Post,
      Headers = headers,
      Body = JsonContent
    };
    _mockHttp
      .Expect(Url)
      .With(message => (bool)message.Content?.Headers.ContentType?.MediaType?.Equals(JsonContentType, StringComparison.OrdinalIgnoreCase))
      .Respond(HttpStatusCode.NoContent);

    // act
    await sut.Run(request);

    // assert
    _mockHttp.VerifyNoOutstandingExpectation();
  }
}

