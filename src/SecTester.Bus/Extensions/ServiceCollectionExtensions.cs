using System;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using SecTester.Bus.Dispatchers;
using SecTester.Bus.RetryStrategies;
using SecTester.Core;
using SecTester.Core.Bus;

namespace SecTester.Bus.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSecTesterBus(this IServiceCollection collection)
  {
    collection
      .AddHttpCommandDispatcher()
      .AddSingleton(_ => new ExponentialBackoffOptions())
      .AddSingleton<RetryStrategy, ExponentialBackoffRetryStrategy>()
      .AddSingleton<RmqEventBusFactory, DefaultRmqEventBusFactory>();

    return collection;
  }

  private static IServiceCollection AddHttpCommandDispatcher(this IServiceCollection collection)
  {
    return collection.AddScoped(sp =>
      {
        var config = sp.GetRequiredService<Configuration>();
        return new HttpCommandDispatcherConfig(config.Api, config.Credentials!.Token, TimeSpan.FromSeconds(10));
      })
      .AddScoped<HttpCommandDispatcher>()
      .AddScoped<CommandDispatcher>(sp => sp.GetRequiredService<HttpCommandDispatcher>())
      .AddHttpClient();
  }

  private static IServiceCollection AddHttpClient(this IServiceCollection collection)
  {
    collection.AddHttpClient(nameof(HttpCommandDispatcher), (sp, client) =>
      {
        var config = sp.GetRequiredService<HttpCommandDispatcherConfig>();
        client.Timeout = (TimeSpan)config.Timeout!;
      })
      .ConfigurePrimaryHttpMessageHandler(_ =>
        new RateLimitedHandler(new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
          Window = TimeSpan.FromSeconds(60),
          SegmentsPerWindow = 6,
          PermitLimit = 10
        }))
      );
    return collection;
  }
}
