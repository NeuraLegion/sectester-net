using Microsoft.Extensions.DependencyInjection;

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
  }
}
