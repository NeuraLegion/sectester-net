using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SecTester.Core;
using SecTester.Core.Logger;
using SecTester.Repeater.Bus;

namespace SecTester.Repeater;

public class DefaultRepeaterFactory : IRepeaterFactory
{
  private readonly Configuration _configuration;
  private readonly IRepeaterBusFactory _busFactory;
  private readonly ILoggerFactory _loggerFactory;
  private readonly IAnsiCodeColorizer _ansiCodeColorizer;
  private readonly RequestRunnerResolver _requestRunnerResolver;

  public DefaultRepeaterFactory(IRepeaterBusFactory busFactory, Configuration configuration, ILoggerFactory loggerFactory, IAnsiCodeColorizer ansiCodeColorizer, RequestRunnerResolver requestRunnerResolver)
  {
    _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    _busFactory = busFactory ?? throw new ArgumentNullException(nameof(busFactory));
    _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    _ansiCodeColorizer = ansiCodeColorizer ?? throw new ArgumentNullException(nameof(ansiCodeColorizer));
    _requestRunnerResolver = requestRunnerResolver ?? throw new ArgumentNullException(nameof(requestRunnerResolver));
  }

  public Task<IRepeater> CreateRepeater(RepeaterOptions? options = default)
  {
    options ??= new RepeaterOptions();

    var bus = _busFactory.Create(options.NamePrefix);
    var version = new Version(_configuration.RepeaterVersion);

    return Task.FromResult<IRepeater>(new Repeater(bus, version, _loggerFactory.CreateLogger<Repeater>(), _ansiCodeColorizer, _requestRunnerResolver));
  }
}
