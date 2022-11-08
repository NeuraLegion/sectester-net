using System;
using Microsoft.Extensions.DependencyInjection;

namespace SecTester.Core.Extensions;

public static class ServiceProviderExtensions
{
  public static T ResolveWith<T>(this IServiceProvider provider, params object[] parameters) where T : class
  {
    return ActivatorUtilities.CreateInstance<T>(provider, parameters);
  }
}
