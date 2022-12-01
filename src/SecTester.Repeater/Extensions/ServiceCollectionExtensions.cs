using System;
using Microsoft.Extensions.DependencyInjection;
using SecTester.Core.Utils;
using SecTester.Repeater.Api;

namespace SecTester.Repeater.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSecTesterRepeater(this IServiceCollection collection)
  {
    return AddSecTesterRepeater(collection, () => new RepeaterOptions());
  }

  public static IServiceCollection AddSecTesterRepeater(this IServiceCollection collection, Func<RepeaterOptions> configure)
  {
    collection
      .AddScoped<RepeaterFactory, DefaultRepeaterFactory>()
      .AddScoped(_ => configure())
      .AddScoped<Repeaters, DefaultRepeaters>()
      .AddScoped<TimerProvider, SystemTimerProvider>();

    return collection;
  }
}
