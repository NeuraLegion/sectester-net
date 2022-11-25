namespace SecTester.Bus.Tests.Extensions;

public class HttpResponseMessageExtensionsTests
{
  private const string CustomErrorMessage = "Custom Error Message";

  public static readonly IEnumerable<object[]> SucceededStatusCodes = new List<object[]>
  {
    new object[] { HttpStatusCode.OK },
    new object[] { HttpStatusCode.Created },
    new object[] { HttpStatusCode.Accepted }
  };

  public static readonly IEnumerable<object[]> UnsuccessfulStatusCodes = new List<object[]>
  {
    new object[] { HttpStatusCode.Redirect },
    new object[] { HttpStatusCode.BadRequest },
    new object[] { HttpStatusCode.BadGateway }
  };

  public static readonly IEnumerable<object[]> CustomErrorMessageConditions = new List<object[]>
  {
    new object[] { "text/html", CustomErrorMessage },
    new object[] { "text/html", $"<html><head></head><body>{CustomErrorMessage}</body></html>" }
  };

  public static readonly IEnumerable<object[]> DefaultErrorMessageConditions = new List<object[]>
  {
    new object[] { "application/json", CustomErrorMessage },
    new object[] { "text/html", CustomErrorMessage + new string('.', 8192) },
    new object[] { "text/html", "" },
    new object[] { "text/html", new string(' ', 4) }
  };

  [Theory]
  [MemberData(nameof(SucceededStatusCodes))]
  public void VerifySuccessStatusCode_GivenStatusCode_Returns(HttpStatusCode httpStatusCode)
  {
    // arrange 
    var message = new HttpResponseMessage(httpStatusCode);

    // act
    var result = message.VerifySuccessStatusCode();

    // assert
    result.Should().Be(message);
  }

  [Theory]
  [MemberData(nameof(UnsuccessfulStatusCodes))]
  public void VerifySuccessStatusCode_GivenStatusCode_ThrowError(HttpStatusCode httpStatusCode)
  {
    // arrange 
    var message = new HttpResponseMessage(httpStatusCode);

    // act
    var act = () => message.VerifySuccessStatusCode();

    // assert
    act.Should().Throw<HttpRequestException>();
  }


  [Theory]
  [MemberData(nameof(CustomErrorMessageConditions))]
  public void VerifySuccessStatusCode_ThrowErrorWithCustomMessage(string contentType, string content)
  {
    // arrange 
    var message = new HttpResponseMessage(HttpStatusCode.BadRequest)
    {
      Content = new StringContent(content, Encoding.UTF8, contentType)
    };

    // act
    var act = () => message.VerifySuccessStatusCode();

    // assert
    act.Should().Throw<HttpRequestException>().WithMessage(
      $"{CustomErrorMessage}: 400 (Bad Request).");
  }

  [Theory]
  [MemberData(nameof(DefaultErrorMessageConditions))]
  public void VerifySuccessStatusCode_ThrowErrorWithDefaultMessage(string contentType, string content)
  {
    // arrange 
    var message = new HttpResponseMessage(HttpStatusCode.BadRequest)
    {
      Content = new StringContent(content, Encoding.UTF8, contentType)
    };

    // act
    var act = () => message.VerifySuccessStatusCode();

    // assert
    act.Should().Throw<HttpRequestException>().WithMessage(
      "*400 (Bad Request).*").And.Message.Should().NotContain(CustomErrorMessage);
  }
}
