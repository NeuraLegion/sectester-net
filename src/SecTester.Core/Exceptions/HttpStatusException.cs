using System.Net;
using System.Net.Http;

namespace SecTester.Core.Exceptions;

public class HttpStatusException : HttpRequestException
{
  public HttpStatusCode StatusCode { get; }
  public bool Retryable => StatusCode >= HttpStatusCode.InternalServerError;

  public HttpStatusException(string message, HttpStatusCode statusCode) : base(message)
  {
    StatusCode = statusCode;
  }
}
