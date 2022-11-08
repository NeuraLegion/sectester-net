using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Bus.Commands;
using SecTester.Core.Bus;

namespace SecTester.Bus.Dispatchers;

public class HttpCommandDispatcher : CommandDispatcher
{
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly HttpCommandDispatcherConfig _config;

  private readonly HttpMethod[] _methodsForbidBody =
  {
    HttpMethod.Head, HttpMethod.Options, HttpMethod.Get, HttpMethod.Trace
  };

  public HttpCommandDispatcher(IHttpClientFactory httpClientFactory, HttpCommandDispatcherConfig config)
  {
    _config = config;
    _httpClientFactory = httpClientFactory;
  }

  public async Task<TResult?> Execute<TResult>(Command<TResult> message)
  {
    using var cts = new CancellationTokenSource();
    cts.CancelAfter(TimeSpan.FromMilliseconds(message.Ttl));
    var res = await PerformHttpRequest((HttpRequest<TResult>)message, cts.Token);

    if (message.ExpectReply)
    {
      return await ParserResponse<TResult>(res);
    }

    res.Content.Dispose();
    return default;
  }

  private async Task<TResult?> ParserResponse<TResult>(HttpResponseMessage res)
  {
    var responseBody = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
    var data = JsonSerializer.Deserialize<TResult>(responseBody,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
      });

    return data;
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
    var content = request.Body != null && !_methodsForbidBody.Contains(request.Method)
      ? new StringContent(request.Body, Encoding.UTF8, "application/json")
      : null;
    var options = new HttpRequestMessage
    {
      RequestUri = CreateUri(request.Url),
      Method = request.Method,
      Content = content,
      Headers = { Authorization = new AuthenticationHeaderValue("api-key", _config.Token), Date = request.CreatedAt }
    };
    options.Headers.Add("x-correlation-id", request.CorrelationId);
    return options;
  }

  private Uri CreateUri(string uri)
  {
    var baseUri = new Uri(_config.BaseUrl);

    return string.IsNullOrEmpty(uri) ? baseUri : new Uri(baseUri, uri);
  }
}
