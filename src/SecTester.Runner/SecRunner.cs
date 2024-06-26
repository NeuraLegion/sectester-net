using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SecTester.Core;
using SecTester.Core.Extensions;
using SecTester.Repeater;
using SecTester.Repeater.Api;
using SecTester.Repeater.Extensions;
using SecTester.Reporter;
using SecTester.Scan;
using SecTester.Scan.Extensions;

namespace SecTester.Runner;

public class SecRunner : IAsyncDisposable
{
  private readonly Configuration _configuration;
  private readonly IFormatter _formatter;
  private readonly IRepeaterFactory _repeaterFactory;
  private readonly IRepeaters _repeatersManager;
  private readonly IScanFactory _scanFactory;
  private IRepeater? _repeater;

  public SecRunner(Configuration configuration, IRepeaterFactory repeaterFactory, IScanFactory scanFactory, IRepeaters repeatersManager,
    IFormatter formatter)
  {
    _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    _repeaterFactory = repeaterFactory ?? throw new ArgumentNullException(nameof(repeaterFactory));
    _scanFactory = scanFactory ?? throw new ArgumentNullException(nameof(scanFactory));
    _repeatersManager = repeatersManager ?? throw new ArgumentNullException(nameof(repeatersManager));
    _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
  }

  public string? RepeaterId => _repeater?.RepeaterId;

  public async ValueTask DisposeAsync()
  {
    if (_repeater is not null)
    {
      await Clear().ConfigureAwait(false);
    }

    GC.SuppressFinalize(this);
  }

  public static async Task<SecRunner> Create(Configuration configuration)
  {
    await configuration.LoadCredentials().ConfigureAwait(false);

    var collection = new ServiceCollection()
      .AddSecTesterConfig(configuration)
      .AddSecTesterRepeater()
      .AddSecTesterScan()
      .AddScoped<IFormatter, DefaultFormatter>()
      .AddScoped<SecRunner>();

    var sp = collection.BuildServiceProvider();

    return sp.GetRequiredService<SecRunner>();
  }

  public async Task Init(RepeaterOptions? options = default, CancellationToken cancellationToken = default)
  {
    if (_repeater is not null)
    {
      throw new InvalidOperationException("Already initialized.");
    }

    await _configuration.LoadCredentials().ConfigureAwait(false);

    _repeater = await _repeaterFactory.CreateRepeater(options).ConfigureAwait(false);
    await _repeater.Start(cancellationToken).ConfigureAwait(false);
  }

  public async Task Clear(CancellationToken cancellationToken = default)
  {
    try
    {
      if (_repeater is not null)
      {
        await _repeater.Stop(cancellationToken).ConfigureAwait(false);
        await _repeatersManager.DeleteRepeater(_repeater.RepeaterId).ConfigureAwait(false);
        await _repeater.DisposeAsync().ConfigureAwait(false);
      }
    }
    catch
    {
      // noop
    }
    finally
    {
      _repeater = null;
    }
  }

  public SecScan CreateScan(ScanSettingsBuilder builder)
  {
    if (_repeater is null)
    {
      throw new InvalidOperationException("Must be initialized first.");
    }

    return new SecScan(
      builder.WithRepeater(_repeater.RepeaterId),
      _scanFactory,
      _formatter
    );
  }
}
