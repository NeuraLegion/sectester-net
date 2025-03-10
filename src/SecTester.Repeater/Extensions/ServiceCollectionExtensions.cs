using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using SecTester.Core.Extensions;
using SecTester.Repeater.Api;
using SecTester.Repeater.Bus;
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
      .AddSingleton<IRepeaterBusFactory, DefaultRepeaterBusFactory>()
      .AddScoped<IRepeaterFactory, DefaultRepeaterFactory>()
      .AddScoped<IRepeaters, DefaultRepeaters>()
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


}
