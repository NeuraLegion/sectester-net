using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SecTester.Bus.Extensions;

internal static class HttpResponseMessageExtensions
{
  private const int ContentLengthThreshold = 4096;
  private const string DetailsContentType = "text/html";
  private const string StripHtmlTagsRegex = "<[^>]*>";

  public static HttpResponseMessage VerifySuccessStatusCode(this HttpResponseMessage httpResponseMessage)
  {
    if (httpResponseMessage.CanHandle())
    {
      var message = httpResponseMessage.TryExtractMessage();

      if (message is not null)
      {
        throw new HttpRequestException(
          $"{message}: {(int)httpResponseMessage.StatusCode} ({httpResponseMessage.ReasonPhrase}).");
      }
    }

    return httpResponseMessage.EnsureSuccessStatusCode();
  }

  private static bool CanHandle(this HttpResponseMessage httpResponseMessage)
  {
    return (int)httpResponseMessage.StatusCode >= 400 &&
           httpResponseMessage.Content.Headers.ContentType is not null &&
           httpResponseMessage.Content.Headers.ContentLength is > 0 and < ContentLengthThreshold &&
           DetailsContentType.Equals(httpResponseMessage.Content.Headers.ContentType.MediaType,
             StringComparison.OrdinalIgnoreCase);
  }

  private static string? TryExtractMessage(this HttpResponseMessage httpResponseMessage)
  {
    var content = Task.Run(() => httpResponseMessage.Content.ReadAsStringAsync())
      .ConfigureAwait(false).GetAwaiter().GetResult();

    if (!string.IsNullOrWhiteSpace(content))
    {
      return Regex.Replace(content, StripHtmlTagsRegex, string.Empty);
    }

    return default;
  }
}
