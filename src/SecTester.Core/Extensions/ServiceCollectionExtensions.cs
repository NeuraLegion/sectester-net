using System;
using System.Net.Http;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SecTester.Core.Bus;
using SecTester.Core.Dispatchers;
using SecTester.Core.Logger;
using SecTester.Core.RetryStrategies;
using SecTester.Core.Utils;

namespace SecTester.Core.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSecTesterConfig(this IServiceCollection collection, string hostname)
  {
    return collection.AddSecTesterConfig(new Configuration(hostname));
  }

  public static IServiceCollection AddSecTesterConfig(this IServiceCollection collection, Configuration configuration) =>
    collection
      .AddSingleton(configuration)
      .AddSingleton<ISystemTimeProvider>(new UtcSystemTimeProvider())
      .AddSingleton<IAnsiCodeColorizer>(new DefaultAnsiCodeColorizer(ConsoleUtils.IsColored))
      .AddLogging(builder =>
      {
        builder.SetMinimumLevel(configuration.LogLevel)
          .AddConsole(options =>
          {
            options.LogToStandardErrorThreshold = LogLevel.Error;
            options.FormatterName = nameof(ColoredConsoleFormatter);
          })
          .AddConsoleFormatter<ColoredConsoleFormatter, ConsoleFormatterOptions>(
            options =>
            {
              options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
              options.UseUtcTimestamp = false;
              options.IncludeScopes = false;
            }
          );
      });

  public static IServiceCollection AddHttpCommandDispatcher(this IServiceCollection collection) =>
    collection
      .AddSingleton(new ExponentialBackoffOptions())
      .AddSingleton<IRetryStrategy, ExponentialBackoffIRetryStrategy>()
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
