namespace SecTester.Bus.Tests.Extensions;

public class HttpResponseMessageExtensionsTests
{
  private const string CustomErrorMessage = "Custom Error Message";
  private const string DefaultErrorMessagePattern = "Request failed with status code *";

  public static readonly IEnumerable<object[]> SucceededStatusCodes = new List<object[]>
  {
    new object[]
    {
      HttpStatusCode.OK
    },
    new object[]
    {
      HttpStatusCode.Created
    },
    new object[]
    {
      HttpStatusCode.Accepted
    }
  };

  public static readonly IEnumerable<object[]> UnsuccessfulStatusCodes = new List<object[]>
  {
    new object[]
    {
      HttpStatusCode.Redirect
    },
    new object[]
    {
      HttpStatusCode.BadRequest
    },
    new object[]
    {
      HttpStatusCode.BadGateway
    }
  };

  public static readonly IEnumerable<object[]> ExpectedConditions = new List<object[]>
  {
    new object[]
    {
      "text/html", CustomErrorMessage
    },
    new object[]
    {
      "text/plain", CustomErrorMessage
    }
  };

  public static readonly IEnumerable<object[]> NonExpectedConditions = new List<object[]>
  {
    new object[]
    {
      "application/json", @"{""foo"":""bar""}"
    },
    new object[]
    {
      "text/plain", ""
    }
  };

  [Theory]
  [MemberData(nameof(SucceededStatusCodes))]
  public async Task ThrowIfUnsuccessful_SuccessfulStatusCode_NotThrows(HttpStatusCode httpStatusCode)
  {
    // arrange 
    var message = new HttpResponseMessage(httpStatusCode);

    // act
    var act = () => message.ThrowIfUnsuccessful();

    // assert
    await act.Should().NotThrowAsync<HttpStatusException>();
  }

  [Theory]
  [MemberData(nameof(UnsuccessfulStatusCodes))]
  public async Task ThrowIfUnsuccessful_UnsuccessfulStatusCode_ThrowsDefaultError(HttpStatusCode httpStatusCode)
  {
    // arrange 
    var message = new HttpResponseMessage(httpStatusCode);

    // act
    var act = () => message.ThrowIfUnsuccessful();

    // assert
    await act.Should().ThrowAsync<HttpRequestException>();
  }

  [Theory]
  [MemberData(nameof(ExpectedConditions))]
  public async Task ThrowIfUnsuccessful_UnsuccessfulStatusCodeWithErrorMessage_ThrowsError(string contentType, string content)
  {
    // arrange 
    var message = new HttpResponseMessage(HttpStatusCode.BadRequest)
    {
      Content = new StringContent(content, Encoding.UTF8, contentType)
    };

    // act
    var act = () => message.ThrowIfUnsuccessful();

    // assert
    await act.Should().ThrowAsync<HttpStatusException>().WithMessage(
      CustomErrorMessage);
  }

  [Theory]
  [MemberData(nameof(NonExpectedConditions))]
  public async Task ThrowIfUnsuccessful_UnsuccessfulStatusCodeWithErrorMessage_ThrowsDefaultError(string contentType, string content)
  {
    // arrange 
    var message = new HttpResponseMessage(HttpStatusCode.BadRequest)
    {
      Content = new StringContent(content, Encoding.UTF8, contentType)
    };

    // act
    var act = () => message.ThrowIfUnsuccessful();

    // assert
    await act.Should().ThrowAsync<HttpStatusException>().WithMessage(
      DefaultErrorMessagePattern);
  }
}
