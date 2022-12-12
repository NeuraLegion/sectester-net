using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecTester.Core;
using SecTester.Core.Utils;
using SecTester.Repeater.Api;
using SecTester.Repeater.Bus;

namespace SecTester.Repeater;

public class DefaultRepeaterFactory : RepeaterFactory
{
  private readonly Configuration _configuration;
  private readonly RepeaterEventBusFactory _eventBusFactory;
  private readonly ILogger _logger;
  private readonly Repeaters _repeaters;
  private readonly IServiceScopeFactory _serviceScopeFactory;

  public DefaultRepeaterFactory(IServiceScopeFactory serviceScopeFactory, Repeaters repeaters, RepeaterEventBusFactory eventBusFactory, Configuration configuration, ILogger logger)
  {
    _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    _repeaters = repeaters ?? throw new ArgumentNullException(nameof(repeaters));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _eventBusFactory = eventBusFactory ?? throw new ArgumentNullException(nameof(eventBusFactory));
    _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
  }

  public async Task<IRepeater> CreateRepeater(RepeaterOptions? options = default)
  {
    options ??= new RepeaterOptions();
    Version version = new(_configuration.Version);

    string repeaterId = await _repeaters.CreateRepeater($"{options.NamePrefix}-{Guid.NewGuid()}", options.Description).ConfigureAwait(false);
    var eventBus = _eventBusFactory.Create(repeaterId);

    var scope = _serviceScopeFactory.CreateAsyncScope();
    await using var _ = scope.ConfigureAwait(false);
    var timerProvider = scope.ServiceProvider.GetRequiredService<TimerProvider>();

    return new Repeater(repeaterId, eventBus, version, _logger, timerProvider);
  }
}
