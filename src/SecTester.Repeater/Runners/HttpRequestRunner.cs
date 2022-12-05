using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Repeater.Bus;

namespace SecTester.Repeater.Runners;

internal class HttpRequestRunner : RequestRunner
{
  private const string DefaultMimeType = "text/plain";

  private const string ContentLengthFieldName = "Content-Length";
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly RequestRunnerOptions _options;

  public HttpRequestRunner(RequestRunnerOptions options, IHttpClientFactory httpClientFactory)
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
      var response = await Request(options, cts.Token).ConfigureAwait(false);
      return await CreateRequestExecutingResult(response).ConfigureAwait(false);
    }
    catch (Exception err)
    {
      return await CreateRequestExecutingResult(err).ConfigureAwait(false);
    }
  }

  private static Task<RequestExecutingResult> CreateRequestExecutingResult(Exception response)
  {
    return Task.FromResult(new RequestExecutingResult
    {
      Message = response.Message,
      // TODO: use native errno codes instead
      ErrorCode = response is SocketException exception ? Enum.GetName(typeof(SocketError), exception.SocketErrorCode) : null
    });
  }

  private async Task<RequestExecutingResult> CreateRequestExecutingResult(HttpResponseMessage response)
  {
    var body = await TruncateResponseBody(response).ConfigureAwait(false);
    var headers = AggregateHeaders(response, body.Length);

    return new RequestExecutingResult
    {
      Headers = headers,
      StatusCode = (int)response.StatusCode,
      Body = Encoding.UTF8.GetString(body)
    };
  }

  private static IEnumerable<KeyValuePair<string, IEnumerable<string>>> AggregateHeaders(HttpResponseMessage response, int contentLength)
  {

    var headers = response.Headers.ToList();
    headers.AddRange(response.Content.Headers);

    var contentLenghtIdx = headers.FindIndex(x => x.Key.Equals(ContentLengthFieldName, StringComparison.OrdinalIgnoreCase));
    if (contentLenghtIdx != -1)
    {
      headers[contentLenghtIdx] = new KeyValuePair<string, IEnumerable<string>>(ContentLengthFieldName, new[]
      {
        $"{contentLength}"
      });
    }

    return headers;
  }

  private async Task<byte[]> TruncateResponseBody(HttpResponseMessage response)
  {
    if (response.StatusCode == HttpStatusCode.NoContent || response.RequestMessage.Method == HttpMethod.Head || response.Content == null)
    {
      return Array.Empty<byte>();
    }

    var type = response.Content.Headers.ContentType.MediaType ?? DefaultMimeType;
    var allowed = _options.AllowedMimes.Any(mime => type.Contains(mime));

    return await ParseResponseBody(response, allowed).ConfigureAwait(false);
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

  private static HttpRequestMessage CreateHttpRequestMessage(Request request)
  {
    var content = new StringContent(request.Body ?? "", Encoding.Default);
    var options = new HttpRequestMessage
    {
      RequestUri = request.Url,
      Method = request.Method,
      Content = content
    };

    foreach (var keyValuePair in request.Headers)
    {
      options.Headers.Add(keyValuePair.Key, keyValuePair.Value);
    }

    return options;
  }

  private async Task<HttpResponseMessage> Request(HttpRequestMessage options, CancellationToken cancellationToken = default)
  {
    using var httpClient = _httpClientFactory.CreateClient(nameof(HttpRequestRunner));
    return await httpClient.SendAsync(options,
      cancellationToken).ConfigureAwait(false);
  }
}
