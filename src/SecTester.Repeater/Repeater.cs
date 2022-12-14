using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SecTester.Core.Bus;
using SecTester.Core.Exceptions;
using SecTester.Core.Extensions;
using SecTester.Core.Logger;
using SecTester.Core.Utils;
using SecTester.Repeater.Bus;

namespace SecTester.Repeater;

public class Repeater : IRepeater
{
  private static readonly TimeSpan DefaultPingInterval = TimeSpan.FromSeconds(10);
  private readonly IEventBus _eventBus;
  private readonly ITimerProvider _heartbeat;
  private readonly ILogger _logger;
  private readonly SemaphoreSlim _semaphore = new(1, 1);
  private readonly Version _version;
  private readonly IAnsiCodeColorizer _ansiCodeColorizer;

  public Repeater(string repeaterId, IEventBus eventBus, Version version, ILogger<Repeater> logger, ITimerProvider heartbeat,
    IAnsiCodeColorizer ansiCodeColorizer)
  {
    RepeaterId = repeaterId ?? throw new ArgumentNullException(nameof(repeaterId));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _version = version ?? throw new ArgumentNullException(nameof(version));
    _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    _heartbeat = heartbeat ?? throw new ArgumentNullException(nameof(heartbeat));
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

      await Register().ConfigureAwait(false);
      SubscribeToEvents();
      await SchedulePing().ConfigureAwait(false);

      Status = RunningStatus.Running;
    }
    catch
    {
      Status = RunningStatus.Off;
      throw;
    }
  }

  private void SubscribeToEvents()
  {
    _eventBus.Register<RequestExecutingEventListener, RequestExecutingEvent, RequestExecutingResult>();
  }

  private async Task SchedulePing()
  {
    await SendStatus(RepeaterStatus.Connected).ConfigureAwait(false);
    _heartbeat.Interval = DefaultPingInterval.TotalMilliseconds;
    _heartbeat.Elapsed += async (_, _) => await SendStatus(RepeaterStatus.Connected).ConfigureAwait(false);
    _heartbeat.Start();
  }

  private async Task SendStatus(RepeaterStatus status)
  {
    var @event = new RepeaterStatusEvent(RepeaterId, status);
    await _eventBus.Publish(@event).ConfigureAwait(false);
  }

  private async Task Register()
  {
    var command = new RegisterRepeaterCommand(_version.ToString(), RepeaterId);
    var res = await _eventBus.Execute(command).ConfigureAwait(false);

    if (res == null)
    {
      throw new SecTesterException("Error registering repeater.");
    }

    EnsureRegistrationStatus(res.Payload);
  }

  public async Task Stop(CancellationToken cancellationToken = default)
  {
    using var _ = await _semaphore.LockAsync(cancellationToken).ConfigureAwait(false);

    try
    {
      if (Status != RunningStatus.Running)
      {
        return;
      }

      Status = RunningStatus.Off;

      _heartbeat.Stop();
      await SendStatus(RepeaterStatus.Disconnected).ConfigureAwait(false);
      _eventBus.Dispose();
    }
    catch
    {
      // noop
    }
  }

  private void EnsureRegistrationStatus(RegisterRepeaterPayload result)
  {
    if (result.Error != RepeaterRegisteringError.None)
    {
      HandleRegisterError(result.Error);
    }
    else
    {
      if (new Version(result.Version!).CompareTo(_version) != 0)
      {
        _logger.LogWarning("{Prefix}: A new Repeater version ({Version}) is available, please update SecTester",
          _ansiCodeColorizer.Colorize(AnsiCodeColor.Yellow, "(!) IMPORTANT"), result.Version);
      }
    }
  }

  private void HandleRegisterError(RepeaterRegisteringError error)
  {
    throw error switch
    {
      RepeaterRegisteringError.NotActive => new SecTesterException("Access Refused: The current Repeater is not active."),
      RepeaterRegisteringError.NotFound => new SecTesterException("Unauthorized access. Please check your credentials."),
      RepeaterRegisteringError.Busy => new SecTesterException(
        $"Access Refused: There is an already running Repeater with ID {RepeaterId}"),
      RepeaterRegisteringError.RequiresToBeUpdated => new SecTesterException(
        $"{_ansiCodeColorizer.Colorize(AnsiCodeColor.Red, "(!) CRITICAL")}: The current running version is no longer supported, please update SecTester."),
      _ => new ArgumentOutOfRangeException(nameof(error), error, "Something went wrong. Unknown error.")
    };
  }
}
