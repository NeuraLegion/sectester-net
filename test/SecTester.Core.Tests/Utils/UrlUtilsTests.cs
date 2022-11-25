namespace SecTester.Core.Tests.Utils;

public class UrlUtilsTests
{
  public static IEnumerable<object[]> Urls = new List<object[]>
  {
    new object[]
    {
      "http://example.com/", "http://example.com"
    },
    new object[]
    {
      "http://example.com//", "http://example.com"
    },
    new object[]
    {
      "//example.com", "https://example.com"
    },
    new object[]
    {
      "example.com/path", "https://example.com/path"
    },
    new object[]
    {
      "https://example.com/path/1/", "https://example.com/path/1/"
    },
    new object[]
    {
      "http://example.com/pets/?offset=10&limit=10", "http://example.com/pets/?limit=10&offset=10"
    },
    new object[]
    {
      "HTTP://example.com", "http://example.com"
    },
    new object[]
    {
      "HTTP://EXAMPLE.COM/PATH", "http://example.com/PATH"
    },
    new object[]
    {
      "http://example.com/?", "http://example.com"
    },
    new object[]
    {
      "http://example.com/foo/bar/../baz", "http://example.com/foo/baz"
    },
    new object[]
    {
      "http://example.com////foo////bar", "http://example.com/foo/bar"
    },
    new object[]
    {
      "https://user:password@example.com", "https://user:password@example.com"
    },
    new object[]
    {
      "HTTP://example.COM////foo////dummy/../bar/?", "http://example.com/foo/bar/"
    }
  };

  public static IEnumerable<object[]> QueryParameters = new List<object[]>
  {
    new object[]
    {
      Array.Empty<KeyValuePair<string, string>>(),
      ""
    },
    new object[]
    {
      new KeyValuePair<string, string>[]
      {
        new("foo", "")
      },
      "foo="
    },
    new object[]
    {
      new KeyValuePair<string, string>[]
      {
        new("foo", "bar")
      },
      "foo=bar"
    },
    new object[]
    {
      new KeyValuePair<string, string>[]
      {
        new("foo", "bar"), new("foo", "baz")
      },
      "foo=bar&foo=baz"
    },
    new object[]
    {
      new KeyValuePair<string, string>[]
      {
        new("foo", "bar"), new("baz", "foo")
      },
      "foo=bar&baz=foo"
    }
  };

  public static IEnumerable<object[]> QueryStrings = new List<object[]>
  {
    new object[]
    {
      "",
      Array.Empty<KeyValuePair<string, string>>()
    },
    new object[]
    {
      "foo=",
      new KeyValuePair<string, string>[]
      {
        new("foo", "")
      }
    },
    new object[]
    {
      "foo=bar",
      new KeyValuePair<string, string>[]
      {
        new("foo", "bar")
      }
    },
    new object[]
    {
      "foo=bar&foo=baz",
      new KeyValuePair<string, string>[]
      {
        new("foo", "bar,baz")
      }
    },
    new object[]
    {
      "foo=bar&baz=foo",
      new KeyValuePair<string, string>[]
      {
        new("foo", "bar"), new("baz", "foo")
      }
    }
  };

  [Theory]
  [MemberData(nameof(Urls))]
  public void Normalize_GivenUrl_ReturnsNormalized(string input, string output)
  {
    // act
    var result = UrlUtils.NormalizeUrl(input);

    // assert
    result.Should().Be(output);
  }

  [Theory]
  [MemberData(nameof(QueryParameters))]
  public void SerializeQuery_GivenParams_ReturnsQueryString(IEnumerable<KeyValuePair<string, string>> input, string output)
  {
    // act
    var result = UrlUtils.SerializeQuery(input);

    // assert
    result.Should().Be(output);
  }

  [Theory]
  [MemberData(nameof(QueryStrings))]
  public void ParseQuery_GivenString_ReturnsParameters(string input, IEnumerable<KeyValuePair<string, string>> output)
  {
    // act
    var result = UrlUtils.ParseQuery(input);

    // assert
    result.Should().BeEquivalentTo(output);
  }
}
