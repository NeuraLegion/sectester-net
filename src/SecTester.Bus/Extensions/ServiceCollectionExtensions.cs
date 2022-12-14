using System;
using System.Net.Http;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using SecTester.Bus.Dispatchers;
using SecTester.Bus.RetryStrategies;
using SecTester.Core;
using SecTester.Core.Bus;

namespace SecTester.Bus.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSecTesterBus(this IServiceCollection collection) =>
    collection
      .AddHttpCommandDispatcher()
      .AddSingleton(new ExponentialBackoffOptions())
      .AddSingleton<RetryStrategy, ExponentialBackoffRetryStrategy>()
      .AddSingleton<RmqEventBusFactory, DefaultRmqEventBusFactory>();

  private static IServiceCollection AddHttpCommandDispatcher(this IServiceCollection collection) =>
    collection
      .AddScoped(sp =>
      {
        var config = sp.GetRequiredService<Configuration>();
        return new HttpCommandDispatcherConfig(config.Api, config.Credentials!.Token, TimeSpan.FromSeconds(10));
      })
      .AddScoped<HttpCommandDispatcher>()
      .AddScoped<CommandDispatcher>(sp => sp.GetRequiredService<HttpCommandDispatcher>())
      .AddHttpClientForHttpCommandDispatcher();

  private static IServiceCollection AddHttpClientForHttpCommandDispatcher(this IServiceCollection collection)
  {
    collection
      .AddHttpClient(nameof(HttpCommandDispatcher), ConfigureHttpClient)
      .ConfigurePrimaryHttpMessageHandler(CreateHttpMessageHandler);

    return collection;
  }

  private static HttpMessageHandler CreateHttpMessageHandler()
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

  private static void ConfigureHttpClient(IServiceProvider sp, HttpClient client)
  {
    var config = sp.GetRequiredService<HttpCommandDispatcherConfig>();
    client.Timeout = (TimeSpan)config.Timeout!;
  }
}
