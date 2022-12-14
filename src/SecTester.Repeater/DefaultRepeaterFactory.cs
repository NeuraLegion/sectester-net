using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecTester.Core;
using SecTester.Core.Logger;
using SecTester.Core.Utils;
using SecTester.Repeater.Api;
using SecTester.Repeater.Bus;

namespace SecTester.Repeater;

public class DefaultRepeaterFactory : RepeaterFactory
{
  private readonly Configuration _configuration;
  private readonly RepeaterEventBusFactory _eventBusFactory;
  private readonly Repeaters _repeaters;
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly ILoggerFactory _loggerFactory;
  private readonly AnsiCodeColorizer _ansiCodeColorizer;

  public DefaultRepeaterFactory(IServiceScopeFactory scopeFactory, Repeaters repeaters, RepeaterEventBusFactory eventBusFactory, Configuration configuration, ILoggerFactory loggerFactory, AnsiCodeColorizer ansiCodeColorizer)
  {
    _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    _repeaters = repeaters ?? throw new ArgumentNullException(nameof(repeaters));
    _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    _eventBusFactory = eventBusFactory ?? throw new ArgumentNullException(nameof(eventBusFactory));
    _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    _ansiCodeColorizer = ansiCodeColorizer ?? throw new ArgumentNullException(nameof(ansiCodeColorizer));
  }

  public async Task<IRepeater> CreateRepeater(RepeaterOptions? options = default)
  {
    options ??= new RepeaterOptions();

    string repeaterId = await _repeaters.CreateRepeater($"{options.NamePrefix}-{Guid.NewGuid()}", options.Description).ConfigureAwait(false);
    var eventBus = _eventBusFactory.Create(repeaterId);

    var scope = _scopeFactory.CreateAsyncScope();
    var timerProvider = scope.ServiceProvider.GetRequiredService<TimerProvider>();
    var version = new Version(_configuration.RepeaterVersion);

    return new Repeater(repeaterId, eventBus, version, _loggerFactory.CreateLogger<Repeater>(), timerProvider, _ansiCodeColorizer);
  }
}
