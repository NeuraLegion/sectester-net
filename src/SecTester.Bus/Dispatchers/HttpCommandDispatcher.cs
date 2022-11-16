using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Bus.Commands;
using SecTester.Core.Bus;

namespace SecTester.Bus.Dispatchers;

public class HttpCommandDispatcher : CommandDispatcher
{
  private readonly IReadOnlyCollection<HttpMethod> _methodsForbidBody =
    new List<HttpMethod>()
    {
      HttpMethod.Head, HttpMethod.Options, HttpMethod.Get, HttpMethod.Trace
    };

  private const string AuthScheme = "api-key";
  private const string CorrelationIdHeaderField = "x-correlation-id";

  private readonly IHttpClientFactory _httpClientFactory;
  private readonly HttpCommandDispatcherConfig _config;
  private readonly MessageSerializer _messageSerializer;

  public HttpCommandDispatcher(IHttpClientFactory httpClientFactory, HttpCommandDispatcherConfig config,
    MessageSerializer messageSerializer)
  {
    _config = config ?? throw new ArgumentNullException(nameof(config));
    _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    _messageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(messageSerializer));
  }

  public async Task<TResult?> Execute<TResult>(Command<TResult> message)
  {
    using var cts = new CancellationTokenSource(message.Ttl);
    var res = await PerformHttpRequest((HttpRequest<TResult>)message, cts.Token).ConfigureAwait(false);

    if (message.ExpectReply)
    {
      return await ParserResponse<TResult>(res).ConfigureAwait(false);
    }

    res.Content.Dispose();
    return default;
  }

  private async Task<TResult?> ParserResponse<TResult>(HttpResponseMessage res)
  {
    var responseBody = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
    return _messageSerializer.Deserialize<TResult>(responseBody);
  }

  private async Task<HttpResponseMessage> PerformHttpRequest<TResult>(HttpRequest<TResult> request, CancellationToken cancellationToken)
  {
    var options = await CreateHttpRequestMessage(request).ConfigureAwait(false);
    using var httpClient = _httpClientFactory.CreateClient(nameof(HttpCommandDispatcher));
    var response = await httpClient.SendAsync(options,
      request.ExpectReply ? HttpCompletionOption.ResponseContentRead : HttpCompletionOption.ResponseHeadersRead,
      cancellationToken).ConfigureAwait(false);

    response.EnsureSuccessStatusCode();

    return response;
  }

  private async Task<HttpRequestMessage> CreateHttpRequestMessage<TResult>(HttpRequest<TResult> request)
  {
    var content = request.Body != null && !_methodsForbidBody.Contains(request.Method) ? request.Body : null;
    var requestUri = await CreateUri(request.Url, request.Params).ConfigureAwait(false);

    var options = new HttpRequestMessage
    {
      Content = content,
      Method = request.Method,
      RequestUri = requestUri,
      Headers =
      {
        Authorization = new AuthenticationHeaderValue(AuthScheme, _config.Token), Date = request.CreatedAt
      }
    };
    options.Headers.Add(CorrelationIdHeaderField, request.CorrelationId);

    return options;
  }

  private Uri CreateUri(string uri)
  {
    var baseUri = new Uri(_config.BaseUrl);

    return string.IsNullOrEmpty(uri) ? baseUri : new Uri(baseUri, uri);
  }

  private Uri CreateUri(string uri, string query)
  {
    var separator = uri.Contains("?") ? "&" : "?";

    return CreateUri(string.Concat(uri, separator, query));
  }

  private async Task<Uri> CreateUri(string uri, IEnumerable<KeyValuePair<string, string>>? query)
  {
    var formUrlEncodedContent = new FormUrlEncodedContent(query ?? Enumerable.Empty<KeyValuePair<string, string>>());
    var queryString = await formUrlEncodedContent.ReadAsStringAsync().ConfigureAwait(false);

    return CreateUri(uri, queryString);

  }
}
