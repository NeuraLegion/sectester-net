using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace SecTester.Scan.Tests.Target;

public class TargetTests
{
  private const string Method = "GET";
  private const string BaseUrl = "https://example.com";
  private const string DefaultHttpVersion = "HTTP/0.9";
  private const string ContentType = "content-type";
  private const string PlainText = "text/plain";
  private const string ApplicationJson = "application/json";
  private const string ApplicationFormUrlencoded = "application/x-www-form-urlencoded";
  private const string Version = "version";
  private const string Cookie = "cookie";

  [Fact]
  public async Task ToHarRequest_ReturnSimpleGetRequest()
  {
    // arrange
    var target = new SecTester.Scan.Target.Target(BaseUrl);

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      Method,
      Url = BaseUrl,
      HttpVersion = DefaultHttpVersion,
      Headers = Array.Empty<Header>(),
      QueryString = Array.Empty<QueryParameter>(),
      Cookies = Array.Empty<Cookie>(),
      HeadersSize = -1,
      BodySize = -1
    });
  }

  [Fact]
  public async Task ToHarRequest_SetsQueryString()
  {
    // arrange
    var target = new SecTester.Scan.Target.Target(BaseUrl).WithQuery(new[]
    {
      new KeyValuePair<string, string>("foo", "bar")
    });

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      Url = $"{BaseUrl}/?foo=bar",
      QueryString = new object[]
      {
        new
        {
          Name = "foo", Value = "bar"
        }
      }
    });
  }

  [Fact]
  public async Task ToHarRequest_QueryStringPassedInUrl_SetsQueryString()
  {
    // arrange
    const string url = $"{BaseUrl}?foo=bar";
    var target = new SecTester.Scan.Target.Target(url);

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      Url = $"{BaseUrl}/?foo=bar",
      QueryString = new object[]
      {
        new
        {
          Name = "foo", Value = "bar"
        }
      }
    });
  }

  [Fact]
  public async Task ToHarRequest_QueryStringPassedTwice_OverridesUrlQueryString()
  {
    // arrange
    const string url = $"{BaseUrl}?foo=bar";
    var target = new SecTester.Scan.Target.Target(url).WithQuery(new[]
    {
      new KeyValuePair<string, string>("bar", "foo")
    });

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      Url = $"{BaseUrl}/?bar=foo",
      QueryString = new object[]
      {
        new
        {
          Name = "bar", Value = "foo"
        }
      }
    });
  }

  [Fact]
  public async Task ToHarRequest_UsesCustomQuerySerializer()
  {
    // arrange
    var target = new SecTester.Scan.Target.Target(BaseUrl).WithQuery(new[]
    {
      new KeyValuePair<string, string>("bar", "foo"), new KeyValuePair<string, string>("bar", "baz")
    }, _ => "bar=foo,baz");

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      Url = $"{BaseUrl}/?bar=foo%2Cbaz",
      QueryString = new object[]
      {
        new
        {
          Name = "bar", Value = "foo"
        },
        new
        {
          Name = "bar", Value = "baz"
        }
      }
    });
  }

  [Fact]
  public async Task ToHarRequest_SetsHeaders()
  {
    // arrange
    var target = new SecTester.Scan.Target.Target(BaseUrl).WithHeaders(new Dictionary<string, IEnumerable<string>>
    {
      [ContentType] = new[]
      {
        ApplicationJson
      }
    });

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      Headers = new object[]
      {
        new
        {
          Name = ContentType, Value = ApplicationJson
        }
      }
    });
  }

  [Fact]
  public async Task ToHarRequest_HeadersWithSameKeys_MergersHeaders()
  {
    // arrange
    var target = new SecTester.Scan.Target.Target(BaseUrl).WithHeaders(new Dictionary<string, IEnumerable<string>>
    {
      [Cookie] = new[]
      {
        "foo=bar", "bar=foo"
      }
    });

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      Headers = new object[]
      {
        new
        {
          Name = Cookie, Value = "foo=bar"
        },
        new
        {
          Name = Cookie, Value = "bar=foo"
        }
      }
    });
  }

  [Fact]
  public async Task ToHarRequest_NormalizesUrl()
  {
    // arrange
    const string url = "HTTPS://EXAMPLE.COM///";
    var target = new SecTester.Scan.Target.Target(url);

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      Url = BaseUrl
    });
  }

  [Fact]
  public async Task ToHarRequest_ReturnsDefaultHttpVersion()
  {
    // arrange
    var target = new SecTester.Scan.Target.Target(BaseUrl);

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      HttpVersion = DefaultHttpVersion
    });
  }

  [Fact]
  public async Task ToHarRequest_ObtainsHttpVersion()
  {
    // arrange
    const string version = "HTTP/1.1";
    var target = new SecTester.Scan.Target.Target(BaseUrl).WithHeaders(new Dictionary<string, IEnumerable<string>>
    {
      [Version] = new[]
      {
        version
      }
    });

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      HttpVersion = version,
      Headers = new object[]
      {
        new
        {
          Name = Version, Value = version
        }
      }
    });
  }

  [Fact]
  public async Task ToHarRequest_ContentTypeIsInHeaders_ParsesJsonBody()
  {
    // arrange
    const string body = @"{""foo"":""bar""}";

    var content = new StringContent(body, Encoding.Default, ApplicationJson);
    var target = new SecTester.Scan.Target.Target(BaseUrl).WithBody(content).WithHeaders(new Dictionary<string, IEnumerable<string>>
    {
      [ContentType] = new[]
      {
        ApplicationJson
      }
    });

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      PostData = new
      {
        Text = body,
        MimeType = content.Headers.ContentType?.ToString(),
        Params = Array.Empty<PostDataParameter>()
      }
    });
  }

  [Fact]
  public async Task ToHarRequest_BodyIsJsonContent_InfersContentType()
  {
    // arrange
    const string expectedBody = @"{""foo"":""bar""}";
    var content = JsonContent.Create(new
    {
      Foo = "bar"
    }, new MediaTypeHeaderValue(ApplicationJson));
    var target = new SecTester.Scan.Target.Target(BaseUrl).WithBody(content);

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      PostData = new
      {
        Text = expectedBody,
        MimeType = ApplicationJson,
        Params = Array.Empty<PostDataParameter>()
      }
    });
  }

  [Fact]
  public async Task ToHarRequest_BodyIsFormUrlEncodedContent_ParsesParams()
  {
    // arrange
    var input = new KeyValuePair<string, string>[]
    {
      new("foo", "bar")
    };
    var content = new FormUrlEncodedContent(input);
    var target = new SecTester.Scan.Target.Target(BaseUrl).WithBody(content);

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      PostData = new
      {
        Text = "foo=bar",
        MimeType = ApplicationFormUrlencoded,
        Params = new object[]
        {
          new
          {
            Name = "foo", Value = "bar"
          }
        }
      }
    });
  }

  [Fact]
  public async Task ToHarRequest_BodyIsFormData_ParsesParams()
  {
    // arrange
    const string fileName = "file.bin";
    const string name = "file";
    const string fileContent = "text";

    var content = new MultipartFormDataContent();
    var field = new StringContent(fileContent, Encoding.UTF8, PlainText);
    content.Add(field, name, fileName);
    var text = await content.ReadAsStringAsync();

    var target = new SecTester.Scan.Target.Target(BaseUrl).WithBody(content);

    // act
    var result = await target.ToHarRequest();

    // assert
    result.Should().BeEquivalentTo(new
    {
      PostData = new
      {
        Text = text,
        MimeType = content.Headers.ContentType?.ToString(),
        Params = new object[]
        {
          new
          {
            Name = name, Value = fileContent, FileName = fileName
          }
        }
      }
    });
  }
}
