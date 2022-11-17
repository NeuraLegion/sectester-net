using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
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
      .AddSingleton<MessageSerializer, DefaultMessageSerializer>()
      .AddTransient(CreateHttpCommandDispatcherConfig)
      .AddTransient(_ => new ExponentialBackoffOptions())
      .AddSingleton<CommandDispatcher, HttpCommandDispatcher>()
      .AddSingleton<RetryStrategy, ExponentialBackoffRetryStrategy>()
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

    return collection;
  }

  private static HttpCommandDispatcherConfig CreateHttpCommandDispatcherConfig(IServiceProvider sp)
  {

    var config = sp.GetService<Configuration>() ??
                 throw new InvalidOperationException("Unable to find a configuration.");
    return new HttpCommandDispatcherConfig(config.Api, config.Credentials!.Token, TimeSpan.FromSeconds(10));
  }

  [ExcludeFromCodeCoverage]
  public static IServiceCollection AddSecTesterBus(this IServiceCollection collection, string clientQueue)
  {
    AddSecTesterBus(collection).AddSingleton(sp => CreateDefaultEventBusOptions(clientQueue, sp))
      .AddSingleton<RmqConnectionManager, DefaultRmqConnectionManager>(CreateRmqConnectionManager)
      .AddSingleton<EventBus, RmqEventBus>(CreateRmqEventBus);

    return collection;
  }

  [ExcludeFromCodeCoverage]
  private static RmqEventBusOptions CreateDefaultEventBusOptions(string clientQueue, IServiceProvider sp)
  {
    var configuration = sp.GetRequiredService<Configuration>();

    return new RmqEventBusOptions(configuration.Bus, "app", "EventBus", clientQueue)
    {
      Username = "bot",
      Password = configuration.Credentials!.Token,
      HeartbeatInterval = TimeSpan.FromSeconds(30),
      ConnectTimeout = TimeSpan.FromSeconds(30)
    };
  }

  [ExcludeFromCodeCoverage]
  private static RmqEventBus CreateRmqEventBus(IServiceProvider sp)
  {
    var configuration = sp.GetRequiredService<RmqEventBusOptions>();
    var connectionManager = sp.GetRequiredService<RmqConnectionManager>();
    var iLifetimeScope = sp.GetRequiredService<IServiceScopeFactory>();
    var logger = sp.GetRequiredService<ILogger<RmqEventBus>>();
    var messageSerializer = sp.GetRequiredService<MessageSerializer>();

    return new RmqEventBus(configuration, connectionManager, logger, iLifetimeScope, messageSerializer);
  }

  [ExcludeFromCodeCoverage]
  private static DefaultRmqConnectionManager CreateRmqConnectionManager(IServiceProvider sp)
  {
    var configuration = sp.GetRequiredService<RmqEventBusOptions>();
    var retryStrategy = sp.GetRequiredService<RetryStrategy>();
    var factory = new ConnectionFactory
    {
      Uri = new Uri(configuration.Url),
      RequestedHeartbeat = configuration.HeartbeatInterval,
      RequestedConnectionTimeout = configuration.ConnectTimeout,
      DispatchConsumersAsync = true,
      Password = configuration.Password,
      UserName = configuration.Username
    };
    var logger = sp.GetRequiredService<ILogger<DefaultRmqConnectionManager>>();

    return new DefaultRmqConnectionManager(factory, logger, retryStrategy);
  }
}
