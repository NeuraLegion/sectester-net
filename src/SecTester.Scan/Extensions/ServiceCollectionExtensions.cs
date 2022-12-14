using Microsoft.Extensions.DependencyInjection;
using SecTester.Scan.CI;

namespace SecTester.Scan.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSecTesterScan(this IServiceCollection collection)
  {
    return collection
      .AddSingleton<CiDiscovery, DefaultCiDiscovery>()
      .AddScoped<IScans, DefaultScans>()
      .AddScoped<IScanFactory, DefaultScanFactory>();
  }
}
