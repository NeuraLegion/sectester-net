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
    collection
      .AddScoped<RepeaterFactory, DefaultRepeaterFactory>()
      .AddScoped<RequestExecutingEventHandler>()
      .AddScoped(_ => configure())
      .AddScoped<Repeaters, DefaultRepeaters>()
      .AddScoped<TimerProvider, SystemTimerProvider>()
      .AddHttpClient(nameof(HttpRequestRunner), (sp, client) =>
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
      })
      .ConfigurePrimaryHttpMessageHandler(sp =>
      {
        var config = sp.GetRequiredService<RequestRunnerOptions>();
        var proxy = config.ProxyUrl is not null ? new WebProxy(config.ProxyUrl) : null;
        // TODO: set unsafe HTTP parser to allow to attack headers. HttpClient does not accept any other characters which violate [rfc7230](https://tools.ietf.org/html/rfc7230#section-3.2.6).
        return new HttpClientHandler
        {
          Proxy = proxy,
          AllowAutoRedirect = false,
          ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true,
          AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
      });
    ;

    return collection;
  }
}
