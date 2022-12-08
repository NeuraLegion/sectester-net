using System.Collections.Specialized;

namespace SecTester.Scan.Tests.CI;

public class VendorMatcherTests
{
  private readonly JsonSerializerOptions _options = new()
  {
    PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public static readonly IEnumerable<object[]> MatchEnvInput = new List<object[]>
  {
    new object[]
    {
      @"{
        ""env"": { ""env"": ""NODE"", ""includes"": ""/app/.heroku/node/bin/node""}
        }",
      new Dictionary<string, string> { { "NODE", "/app/.heroku/node/bin/node/123" } }, true
    },
    new object[]
    {
      @"{
        ""env"": { ""env"": ""NODE"", ""includes"": ""/app/.heroku/node/bin/node""}
        }",
      new Dictionary<string, string> { { "NODE", "/app/.heroku/node/bin/" } }, false
    },
    new object[]
    {
      @"{
        ""env"": { ""env"": ""NODE-1"", ""includes"": ""/app/.heroku/node/bin/node""}
        }",
      new Dictionary<string, string>(), false
    },
    new object[]
    {
      @"{
      ""env"": { ""CI_NAME"": ""sourcehut"" }
      }",
      new Dictionary<string, string> { { "CI_NAME", "sourcehut" } }, true
    },
    new object[]
    {
      @"{
      ""env"": { ""CI_NAME"": ""sourcehut"" }
      }",
      new Dictionary<string, string> { { "CI_NAME", "sourcehut1" } }, false
    },
    new object[]
    {
      @"{
      ""env"": { ""CI_NAME"": ""sourcehut"" }
      }",
      new Dictionary<string, string>(), false
    },
    new object[]
    {
      @"{
      ""env"": { ""any"": [""NOW_BUILDER"", ""VERCEL""] }
      }",
      new Dictionary<string, string> { { "VERCEL", "value" } }, true
    },
    new object[]
    {
      @"{
      ""env"": { ""any"": [""NOW_BUILDER"", ""VERCEL""] }
      }",
      new Dictionary<string, string>(), false
    },
    new object[]
    {
      @"{
      ""env"": [""JENKINS_URL"", ""BUILD_ID""]      
      }",
      new Dictionary<string, string> { { "JENKINS_URL", "jenkins_url" }, { "BUILD_ID", "build_id" } }, true
    },
    new object[]
    {
      @"{
      ""env"": [""JENKINS_URL"", ""BUILD_ID""]      
      }",
      new Dictionary<string, string> { { "JENKINS_URL", "jenkins_url" } }, false
    },
    new object[]
    {
      @"{
      ""env"": [""JENKINS_URL"", ""BUILD_ID""]      
      }",
      new Dictionary<string, string>(), false
    },
    new object[]
    {
      @"{
      ""env"": ""MAGNUM""  
      }",
      new Dictionary<string, string> { { "MAGNUM", "value" } }, true
    },
    new object[]
    {
      @"{
      ""env"": ""MAGNUM""  
      }",
      new Dictionary<string, string>(), false
    }
  };

  public static readonly IEnumerable<object[]> MatchPrInput = new List<object[]>
  {
    new object[]
    {
      @"{
      ""pr"": ""SAIL_PULL_REQUEST_NUMBER""
      }",
      new Dictionary<string, string> { { "SAIL_PULL_REQUEST_NUMBER", "" } }, true
    },
    new object[]
    {
      @"{
      ""pr"": ""SAIL_PULL_REQUEST_NUMBER""
      }",
      new Dictionary<string, string>(), false
    },
    new object[]
    {
      @"{
       ""pr"": { ""CI_BUILD_EVENT"": ""pull_request"" }
      }",
      new Dictionary<string, string> { { "CI_BUILD_EVENT", "pull_request" } }, true
    },
    new object[]
    {
      @"{
       ""pr"": { ""CI_BUILD_EVENT"": ""pull_request"" }
      }",
      new Dictionary<string, string> { { "CI_BUILD_EVENT", "pull_request1" } }, false
    },
    new object[]
    {
      @"{
       ""pr"": { ""CI_BUILD_EVENT"": ""pull_request"" }
      }",
      new Dictionary<string, string>(), false
    },
    
    new object[]
    {
      @"{
       ""pr"": { ""env"": ""TRAVIS_PULL_REQUEST"", ""ne"": ""false"" }
      }",
      new Dictionary<string, string> { { "TRAVIS_PULL_REQUEST", "some" } }, true
    },
    new object[]
    {
      @"{
       ""pr"": { ""env"": ""TRAVIS_PULL_REQUEST"", ""ne"": ""false"" }
      }",
      new Dictionary<string, string> { { "TRAVIS_PULL_REQUEST", "" } }, false
    },
    new object[]
    {
      @"{
       ""pr"": { ""env"": ""TRAVIS_PULL_REQUEST"", ""ne"": ""false"" }
      }",
      new Dictionary<string, string> { { "TRAVIS_PULL_REQUEST", null! } }, false
    },
    new object[]
    {
      @"{
       ""pr"": { ""env"": ""TRAVIS_PULL_REQUEST"", ""ne"": ""false"" }
      }",
      new Dictionary<string, string> (), false
    },    
    new object[]
    {
      @"{
       ""pr"": { ""any"": [""CF_PULL_REQUEST_NUMBER"", ""CF_PULL_REQUEST_ID""] }
      }",
      new Dictionary<string, string> { { "CF_PULL_REQUEST_NUMBER", "" } }, true
    },
    new object[]
    {
      @"{
       ""pr"": { ""any"": [""CF_PULL_REQUEST_NUMBER"", ""CF_PULL_REQUEST_ID""] }
      }",
      new Dictionary<string, string> (), false
    },
  };

    

  [Theory]
  [MemberData(nameof(MatchEnvInput))]
  public void MatchEnvElement_ReturnsExpected(string vendorInput, IDictionary environmentInput, bool expected)
  {
    // arrange
    var sut = new VendorMatcher(environmentInput);
    var vendor = JsonSerializer.Deserialize<Vendor>(vendorInput, _options)!;

    // act
    var result = sut.MatchEnvElement(vendor.Env);

    //assert
    result.Should().Be(expected);
  }

  [Theory]
  [MemberData(nameof(MatchPrInput))]
  public void MatchPrElement_ReturnsExpected(string vendorInput, IDictionary environmentInput, bool expected)
  {
    // arrange
    var sut = new VendorMatcher(environmentInput);
    var vendor = JsonSerializer.Deserialize<Vendor>(vendorInput, _options)!;

    // act
    var result = sut.MatchPrElement(vendor.Pr);

    //assert
    result.Should().Be(expected);
  }

  [Fact]
  public void MatchEnvElement_GivenAnyEnv_Returns()
  {
    // arrange
    var sut = new VendorMatcher(new Dictionary<string, object>());
    var vendors = JsonSerializer.Deserialize<Vendor[]>(
      ResourceUtils.GetEmbeddedResourceContent<DefaultCiDiscovery>("SecTester.Scan.CI.vendors.json"),
      new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

    // act
    var result = vendors!.FirstOrDefault(x=> sut.MatchEnvElement(x.Env));
    
    // assert
    result.Should().BeNull();
  }
  
  [Fact]
  public void MatchPrElement_GivenAnyPr_Returns()
  {
    // arrange
    var sut = new VendorMatcher(new Dictionary<string, object>());
    var vendors = JsonSerializer.Deserialize<Vendor[]>(
      ResourceUtils.GetEmbeddedResourceContent<DefaultCiDiscovery>("SecTester.Scan.CI.vendors.json"),
      new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

    // act
    var result = vendors!.FirstOrDefault(x=> sut.MatchPrElement(x.Pr));
    
    // assert
    result.Should().BeNull();
  }
}
