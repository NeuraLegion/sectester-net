using Microsoft.Extensions.DependencyInjection;
using SecTester.Scan.CI;

namespace SecTester.Reporter.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSecTesterReporter(this IServiceCollection collection)
  {
    return collection
      .AddSingleton<Formatter, DefaultFormatter>();
  }
}
