using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SecTester.Core.Logger;
using SecTester.Core.Utils;

namespace SecTester.Core.Extensions
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddSecTesterConfig(this IServiceCollection collection, string hostname)
    {
      collection.Add(new ServiceDescriptor(typeof(Configuration), new Configuration(hostname)));
      return collection;
    }

    public static IServiceCollection AddSecTesterConfig(this IServiceCollection collection, Configuration configuration)
    {
      collection.Add(new ServiceDescriptor(typeof(Configuration), configuration));
      return collection;
    }

    public static IServiceCollection AddSystemTimeProvider(this IServiceCollection collection, SystemTimeProvider? instance = null)
    {
      collection.AddSingleton(instance ?? new UtcSystemTimeProvider());
      return collection;
    }

    public static IServiceCollection AddDefaultLogging(this IServiceCollection collection, LogLevel logLevel = LogLevel.Error)
    {
      
      collection.AddLogging((builder) =>
      {
        builder.AddConsoleFormatter<ColoredConsoleFormatter, DefaultConsoleFormatterOptions>();
        builder.SetMinimumLevel(logLevel);
        builder.AddConsole(options =>
        {
          options.LogToStandardErrorThreshold = LogLevel.Error;
          options.FormatterName = nameof(ColoredConsoleFormatter);
        });
      });

      return collection;
    }
  }
}
