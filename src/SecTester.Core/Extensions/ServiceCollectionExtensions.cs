using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SecTester.Core.Logger;
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
}
