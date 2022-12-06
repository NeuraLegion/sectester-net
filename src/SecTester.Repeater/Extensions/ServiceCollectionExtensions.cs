using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using SecTester.Core.Utils;
using SecTester.Repeater.Api;
using SecTester.Repeater.Bus;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSecTesterRepeater(this IServiceCollection collection)
  {
    return AddSecTesterRepeater(collection, () => new RequestRunnerOptions());
  }

  public static IServiceCollection AddSecTesterRepeater(this IServiceCollection collection, Func<RequestRunnerOptions> configure)
  {
    return collection
      .AddScoped<RepeaterFactory, DefaultRepeaterFactory>()
      .AddScoped<RequestExecutingEventHandler>()
      .AddScoped(_ => configure())
      .AddScoped<Repeaters, DefaultRepeaters>()
      .AddScoped<TimerProvider, SystemTimerProvider>()
      .AddHttpClientForHttpRequestRunner();
  }

  private static IServiceCollection AddHttpClientForHttpRequestRunner(this IServiceCollection collection)
  {
    collection.AddHttpClient(nameof(HttpRequestRunner), ConfigureHttpClient)
      .ConfigurePrimaryHttpMessageHandler(CreateHttpMessageHandler);

    return collection;
  }

  private static HttpMessageHandler CreateHttpMessageHandler(IServiceProvider sp)
  {
    var config = sp.GetRequiredService<RequestRunnerOptions>();
    var proxy = config.ProxyUrl is not null ? new WebProxy(config.ProxyUrl) : null;

    return new HttpClientHandler
    {
      Proxy = proxy,
      AllowAutoRedirect = false,
      ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true,
      AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    };
  }

  private static void ConfigureHttpClient(IServiceProvider sp, HttpClient client)
  {
    var config = sp.GetRequiredService<RequestRunnerOptions>();

    client.Timeout = config.Timeout;

    foreach (var keyValuePair in config.Headers)
    {
      client.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
    }

    if (!config.ReuseConnection)
    {
      return;
    }

    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
    client.DefaultRequestHeaders.Add("Keep-Alive", config.Timeout.ToString());
  }
}

