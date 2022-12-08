using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SecTester.Bus.Exceptions;

namespace SecTester.Bus.Extensions;

internal static class HttpResponseMessageExtensions
{
  private const string DefaultErrorMessageTemplate = "Request failed with status code {0}";

  private static readonly IEnumerable<string> AllowedContentTypes = new List<string>
  {
    "text/html", "text/plain"
  };

  public static async Task ThrowIfUnsuccessful(this HttpResponseMessage httpResponseMessage)
  {
    if (!httpResponseMessage.IsSuccessStatusCode)
    {
      var ex = await CreateHttpException(httpResponseMessage).ConfigureAwait(false);

      throw ex;
    }
  }

  private static async Task<HttpRequestException> CreateHttpException(HttpResponseMessage httpResponseMessage)
  {
    var message = await ReadMessage(httpResponseMessage).ConfigureAwait(false);

    return new HttpStatusException(message, httpResponseMessage.StatusCode);
  }

  private static async Task<string> ReadMessage(HttpResponseMessage httpResponseMessage)
  {
    var message = CanObtainErrorMessage(httpResponseMessage) ? await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) : "";

    return string.IsNullOrEmpty(message)
      ? string.Format(CultureInfo.InvariantCulture, DefaultErrorMessageTemplate, httpResponseMessage.StatusCode)
      : message;
  }

  private static bool CanObtainErrorMessage(HttpResponseMessage httpResponseMessage)
  {
    var contentType = httpResponseMessage.Content.Headers.ContentType;
    return contentType != null && AllowedContentTypes.Contains(contentType.MediaType, StringComparer.OrdinalIgnoreCase);
  }
}
