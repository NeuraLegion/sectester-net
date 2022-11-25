using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Bus.Commands;
using SecTester.Core.Bus;
using SecTester.Core.Utils;

namespace SecTester.Bus.Dispatchers;

public class HttpCommandDispatcher : CommandDispatcher
{
  private const string AuthScheme = "api-key";
  private const string CorrelationIdHeaderField = "x-correlation-id";
  private readonly HttpCommandDispatcherConfig _config;

  private readonly IHttpClientFactory _httpClientFactory;
  private readonly MessageSerializer _messageSerializer;

  private readonly IReadOnlyCollection<HttpMethod> _methodsForbidBody =
    new List<HttpMethod>
    {
      HttpMethod.Head, HttpMethod.Options, HttpMethod.Get, HttpMethod.Trace
    };

  private readonly RetryStrategy _retryStrategy;

  public HttpCommandDispatcher(IHttpClientFactory httpClientFactory, HttpCommandDispatcherConfig config,
    MessageSerializer messageSerializer, RetryStrategy retryStrategy)
  {
    _config = config ?? throw new ArgumentNullException(nameof(config));
    _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    _messageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(messageSerializer));
    _retryStrategy = retryStrategy ?? throw new ArgumentNullException(nameof(retryStrategy));
  }

  public async Task<TResult?> Execute<TResult>(Command<TResult> message)
  {
    using var cts = new CancellationTokenSource(message.Ttl);
    var res = await _retryStrategy.Acquire(() => PerformHttpRequest((HttpRequest<TResult>)message, cts.Token)).ConfigureAwait(false);

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
    var options = CreateHttpRequestMessage(request);
    using var httpClient = _httpClientFactory.CreateClient(nameof(HttpCommandDispatcher));
    var response = await httpClient.SendAsync(options,
      request.ExpectReply ? HttpCompletionOption.ResponseContentRead : HttpCompletionOption.ResponseHeadersRead,
      cancellationToken).ConfigureAwait(false);

    response.EnsureSuccessStatusCode();

    return response;
  }

  private HttpRequestMessage CreateHttpRequestMessage<TResult>(HttpRequest<TResult> request)
  {
    var content = request.Body != null && !_methodsForbidBody.Contains(request.Method) ? request.Body : null;
    var requestUri = CreateUri(request.Url, request.Params);

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

  private Uri CreateUri(string uri, IEnumerable<KeyValuePair<string, string>>? query)
  {
    var queryString = query is not null ? UrlUtils.SerializeQuery(query) : "";

    return CreateUri(uri, queryString);

  }
}
