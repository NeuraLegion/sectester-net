using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SecTester.Core.Extensions;
using SecTester.Core.Logger;
using SecTester.Repeater.Bus;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater;

public delegate IRequestRunner? RequestRunnerResolver(Protocol key);


public class Repeater : IRepeater
{
  private readonly IRepeaterBus _bus;
  private readonly ILogger _logger;
  private readonly SemaphoreSlim _semaphore = new(1, 1);
  private readonly Version _version;
  private readonly IAnsiCodeColorizer _ansiCodeColorizer;
  private readonly RequestRunnerResolver _requestRunnersAccessor;

  public Repeater(string repeaterId, IRepeaterBus bus, Version version, ILogger<Repeater> logger,
    IAnsiCodeColorizer ansiCodeColorizer, RequestRunnerResolver requestRunnersAccessor)
  {
    RepeaterId = repeaterId ?? throw new ArgumentNullException(nameof(repeaterId));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _version = version ?? throw new ArgumentNullException(nameof(version));
    _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    _requestRunnersAccessor = requestRunnersAccessor ?? throw new ArgumentNullException(nameof(requestRunnersAccessor));
    _ansiCodeColorizer = ansiCodeColorizer ?? throw new ArgumentNullException(nameof(ansiCodeColorizer));
  }

  public RunningStatus Status { get; private set; } = RunningStatus.Off;
  public string RepeaterId { get; }

  public async ValueTask DisposeAsync()
  {
    await Stop().ConfigureAwait(false);

    _semaphore.Dispose();

    GC.SuppressFinalize(this);
  }

  public async Task Start(CancellationToken cancellationToken = default)
  {
    using var _ = await _semaphore.LockAsync(cancellationToken).ConfigureAwait(false);

    try
    {
      if (Status != RunningStatus.Off)
      {
        throw new InvalidOperationException("Repeater is already active.");
      }

      Status = RunningStatus.Starting;

      SubscribeToEvents();

      await _bus.Connect().ConfigureAwait(false);
      await _bus.Deploy(RepeaterId, cancellationToken).ConfigureAwait(false);

      Status = RunningStatus.Running;
    }
    catch
    {
      Status = RunningStatus.Off;
      throw;
    }
  }

  public async Task Stop(CancellationToken cancellationToken = default)
  {
    using var _ = await _semaphore.LockAsync(cancellationToken).ConfigureAwait(false);

    try
    {
      UnsubscribeFromEvents();

      if (Status != RunningStatus.Running)
      {
        return;
      }

      Status = RunningStatus.Off;

      await _bus.DisposeAsync().ConfigureAwait(false);
    }
    catch
    {
      // noop
    }
  }

  public async Task<OutgoingResponse> HandleIncomingRequest(IncomingRequest message)
  {
    var runner = _requestRunnersAccessor(message.Protocol);

    if (runner == null)
    {
      var msg = $"Unsupported protocol {message.Protocol}";
      _logger.LogError(msg);
      return new OutgoingResponse { Message = msg };
    }

    return (OutgoingResponse)await runner.Run(message).ConfigureAwait(false);
  }

  private void SubscribeToEvents()
  {
    _bus.RequestReceived += HandleIncomingRequest;
    _bus.ErrorOccurred += HandleRegisterError;
    _bus.UpgradeAvailable += HandleUpgradeAvailable;
  }

  private void UnsubscribeFromEvents()
  {
    _bus.RequestReceived -= HandleIncomingRequest;
    _bus.ErrorOccurred -= HandleRegisterError;
    _bus.UpgradeAvailable -= HandleUpgradeAvailable;
  }

  private void HandleUpgradeAvailable(Version version)
  {
    if (version.CompareTo(_version) != 0)
    {
      _logger.LogWarning("{Prefix}: A new Repeater version ({Version}) is available, please update SecTester",
        _ansiCodeColorizer.Colorize(AnsiCodeColor.Yellow, "(!) IMPORTANT"), version);
    }
  }

  private void HandleRegisterError(Exception error)
  {
    _logger.LogError("{Prefix}: {Message}", _ansiCodeColorizer.Colorize(AnsiCodeColor.Red, "(!) IMPORTANT"), error.Message);
  }
}
