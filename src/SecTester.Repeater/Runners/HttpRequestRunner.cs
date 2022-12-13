using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Repeater.Bus;
using SecTester.Repeater.Extensions;

namespace SecTester.Repeater.Runners;

internal sealed class HttpRequestRunner : RequestRunner
{
  private const string DefaultMimeType = "text/plain";

  private const string ContentLengthFieldName = "Content-Length";
  private const string ContentTypeFieldName = "Content-Type";
  private readonly HashSet<string> _contentHeaders = new(StringComparer.OrdinalIgnoreCase)
  {
    ContentLengthFieldName, ContentTypeFieldName
  };

  private readonly IHttpClientFactory _httpClientFactory;
  private readonly RequestRunnerOptions _options;

  public HttpRequestRunner(RequestRunnerOptions options, IHttpClientFactory? httpClientFactory)
  {
    _options = options ?? throw new ArgumentNullException(nameof(options));
    _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
  }

  public Protocol Protocol => Protocol.Http;

  public async Task<Response> Run(Request request)
  {
    try
    {
      var options = CreateHttpRequestMessage(request);
      using var cts = new CancellationTokenSource(_options.Timeout);
      using var response = await Request(options, cts.Token).ConfigureAwait(false);
      using var _ = response.Content;

      return await CreateRequestExecutingResult(response).ConfigureAwait(false);
    }
    catch (Exception err)
    {
      return new RequestExecutingResult
      {
        Message = err.Message,
        // TODO: use native errno codes instead
        ErrorCode = err is SocketException exception ? Enum.GetName(typeof(SocketError), exception.SocketErrorCode) : null
      };
    }
  }

  private async Task<RequestExecutingResult> CreateRequestExecutingResult(HttpResponseMessage response)
  {
    var body = await TruncateResponseBody(response).ConfigureAwait(false);
    var headers = AggregateHeaders(response);

    if (body != null)
    {
      var contentLength = new KeyValuePair<string, IEnumerable<string>>(ContentLengthFieldName, new[]
      {
        $"{body.Length}"
      });
      headers.Replace(contentLength, x => x.Key.Equals(ContentLengthFieldName, StringComparison.OrdinalIgnoreCase));
    }

    return new RequestExecutingResult
    {
      Headers = headers,
      StatusCode = (int)response.StatusCode,
      Body = body?.ToString() ?? ""
    };
  }

  private static List<KeyValuePair<string, IEnumerable<string>>> AggregateHeaders(HttpResponseMessage response)
  {
    var headers = response.Headers.ToList();
    headers.AddRange(response.Content.Headers);
    return headers;
  }

  private async Task<ResponseBody?> TruncateResponseBody(HttpResponseMessage response)
  {
    if (response.StatusCode == HttpStatusCode.NoContent || response.RequestMessage.Method == HttpMethod.Head || response.Content == null)
    {
      return null;
    }

    var contentType = response.Content.Headers.ContentType;
    var mimeType = contentType?.MediaType ?? DefaultMimeType;
    var allowed = _options.AllowedMimes.Any(mime => mimeType.Contains(mime));

    var body = await ParseResponseBody(response, allowed).ConfigureAwait(false);

    return new ResponseBody(body, contentType?.CharSet);
  }

  private async Task<byte[]> ParseResponseBody(HttpResponseMessage response, bool allowed)
  {
    var body = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
    var requiresTruncating = _options.MaxContentLength != -1 &&
                             body.Length > _options.MaxContentLength &&
                             !allowed;

    if (requiresTruncating)
    {
      body = body.Take(_options.MaxContentLength).ToArray();
    }

    return body;
  }

  private HttpRequestMessage CreateHttpRequestMessage(Request request)
  {
    var content = request.Body != null ? CreateHttpContent(request) : null;
    var options = new HttpRequestMessage
    {
      RequestUri = request.Url,
      Method = request.Method,
      Content = content
    };

    request.Headers
      .Where(x => !_contentHeaders.Contains(x.Key))
      .ForEach(x => options.Headers.TryAddWithoutValidation(x.Key, x.Value));

    return options;
  }

  private static StringContent? CreateHttpContent(Request request)
  {
    var values = request.Headers
      .Where(header => header.Key.Equals(ContentTypeFieldName, StringComparison.OrdinalIgnoreCase))
      .SelectMany(header => header.Value).ToArray();

    if (!values.Any())
    {
      return null;
    }

    var mime = new ContentType(string.Join(", ", values));
    var encoding = !string.IsNullOrEmpty(mime.CharSet) ? Encoding.GetEncoding(mime.CharSet) : Encoding.Default;

    return new StringContent(request.Body, encoding, mime.MediaType);
  }

  private async Task<HttpResponseMessage> Request(HttpRequestMessage options, CancellationToken cancellationToken = default)
  {
    using var httpClient = _httpClientFactory.CreateClient(nameof(HttpRequestRunner));
    return await httpClient.SendAsync(options, cancellationToken).ConfigureAwait(false);
  }
}


