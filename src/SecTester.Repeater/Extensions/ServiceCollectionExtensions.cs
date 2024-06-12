using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using SecTester.Core;
using SecTester.Core.Bus;
using SecTester.Core.Utils;
using SecTester.Repeater.Api;
using SecTester.Repeater.Bus;
using SecTester.Repeater.Dispatchers;
using SecTester.Repeater.RetryStrategies;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSecTesterRepeater(this IServiceCollection collection) =>
    AddSecTesterRepeater(collection, new RequestRunnerOptions());

  public static IServiceCollection AddSecTesterRepeater(this IServiceCollection collection, RequestRunnerOptions options) =>
    collection
      .AddSingleton(options)
      .AddHttpCommandDispatcher()
      .AddSingleton(new ExponentialBackoffOptions())
      .AddSingleton<IRetryStrategy, ExponentialBackoffIRetryStrategy>()
      .AddSingleton<IRepeaterBusFactory, DefaultRepeaterBusFactory>()
      .AddScoped<IRepeaterFactory, DefaultRepeaterFactory>()
      .AddScoped<IRepeaters, DefaultRepeaters>()
      .AddScoped<ITimerProvider, SystemTimerProvider>()
      .AddScoped<IRequestRunner, HttpRequestRunner>()
      .AddScoped<RequestRunnerResolver>(sp =>
        protocol => sp.GetServices<IRequestRunner>().FirstOrDefault(x => x.Protocol == protocol)
      )
      .AddHttpClientForHttpRequestRunner();

  private static IServiceCollection AddHttpClientForHttpRequestRunner(this IServiceCollection collection)
  {
    collection.AddHttpClient(nameof(HttpRequestRunner), ConfigureHttpClientForHttpRequestRunner)
      .ConfigurePrimaryHttpMessageHandler(CreateHttpMessageHandlerForHttpRequestRunner);

    return collection;
  }

  private static HttpMessageHandler CreateHttpMessageHandlerForHttpRequestRunner(IServiceProvider sp)
  {
    var config = sp.GetRequiredService<RequestRunnerOptions>();
    var proxy = config.ProxyUrl is not null ? new WebProxy(config.ProxyUrl) : null;

    return new HttpClientHandler
    {
      Proxy = proxy,
      AllowAutoRedirect = false,
      ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
      AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    };
  }

  private static void ConfigureHttpClientForHttpRequestRunner(IServiceProvider sp, HttpClient client)
  {
    var config = sp.GetRequiredService<RequestRunnerOptions>();

    client.Timeout = config.Timeout;

    foreach (var keyValuePair in config.Headers)
    {
      client.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
    }

    if (config.ReuseConnection)
    {
      client.DefaultRequestHeaders.Add("Connection", "keep-alive");
      client.DefaultRequestHeaders.Add("Keep-Alive", config.Timeout.ToString());
    }
  }

  private static IServiceCollection AddHttpCommandDispatcher(this IServiceCollection collection) =>
    collection
      .AddScoped(sp =>
      {
        var config = sp.GetRequiredService<Configuration>();
        return new HttpCommandDispatcherConfig(config.Api, config.Credentials!.Token, TimeSpan.FromSeconds(10));
      })
      .AddScoped<HttpCommandDispatcher>()
      .AddScoped<ICommandDispatcher>(sp => sp.GetRequiredService<HttpCommandDispatcher>())
      .AddHttpClientForHttpCommandDispatcher();

  private static IServiceCollection AddHttpClientForHttpCommandDispatcher(this IServiceCollection collection)
  {
    collection
      .AddHttpClient(nameof(HttpCommandDispatcher), ConfigureHttpClientForHttpCommandDispatcher)
      .ConfigurePrimaryHttpMessageHandler(CreateHttpMessageHandlerForHttpCommandDispatcher);

    return collection;
  }

  private static HttpMessageHandler CreateHttpMessageHandlerForHttpCommandDispatcher()
  {
    var options = new FixedWindowRateLimiterOptions
    {
      Window = TimeSpan.FromSeconds(60),
      PermitLimit = 10,
      QueueLimit = 5
    };
    var rateLimiter = new FixedWindowRateLimiter(options);

    return new RateLimitedHandler(rateLimiter);
  }

  private static void ConfigureHttpClientForHttpCommandDispatcher(IServiceProvider sp, HttpClient client)
  {
    var config = sp.GetRequiredService<HttpCommandDispatcherConfig>();
    client.Timeout = (TimeSpan)config.Timeout!;
  }
}
