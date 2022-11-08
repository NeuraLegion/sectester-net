using System;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using SecTester.Bus.Dispatchers;
using SecTester.Core;

namespace SecTester.Bus.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSecTesterBus(this IServiceCollection collection)
  {
    collection
      .AddHttpClient(nameof(HttpCommandDispatcher), (sp, client) =>
      {
        var config = sp.GetService<HttpCommandDispatcherConfig>() ??
                     throw new InvalidOperationException("Unable to find a HTTP dispatcher config.");
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
    collection.AddTransient(sp =>
    {
      var config = sp.GetService<Configuration>() ??
                   throw new InvalidOperationException("Unable to find a configuration.");
      return new HttpCommandDispatcherConfig(config.Api, config.Credentials!.Token, TimeSpan.FromSeconds(10));
    });
    collection.AddSingleton<HttpCommandDispatcher>();
    return collection;
  }
}
