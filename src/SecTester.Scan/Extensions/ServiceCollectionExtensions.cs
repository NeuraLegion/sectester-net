using Microsoft.Extensions.DependencyInjection;
using SecTester.Scan.CI;
using SecTester.Scan.Content;

namespace SecTester.Scan.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSecTesterScan(this IServiceCollection collection)
  {
    collection
      .AddSingleton<CiDiscovery, DefaultCiDiscovery>()
      .AddSingleton<HttpContentFactory, DefaultHttpContentFactory>()
      .AddSingleton<Scans, DefaultScans>();

    return collection;
  }
}
