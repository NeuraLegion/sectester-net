using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SecTester.Core.Bus;
using SecTester.Core.Exceptions;
using SecTester.Core.Extensions;
using SecTester.Core.Utils;
using SecTester.Repeater.Bus;

namespace SecTester.Repeater;

public class Repeater : IAsyncDisposable
{
  private readonly EventBus _eventBus;
  private readonly TimerProvider _heartbeat;
  private readonly ILogger _logger;
  private readonly SemaphoreSlim _semaphore = new(1, 1);
  private readonly Version _version;

  public Repeater(string repeaterId, EventBus eventBus, Version version, ILogger logger, TimerProvider heartbeat)
  {
    RepeaterId = repeaterId ?? throw new ArgumentNullException(nameof(repeaterId));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _version = version ?? throw new ArgumentNullException(nameof(version));
    _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    _heartbeat = heartbeat ?? throw new ArgumentNullException(nameof(heartbeat));
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
      await SchedulePing().ConfigureAwait(false);

      Status = RunningStatus.Running;
    }
    catch
    {
      Status = RunningStatus.Off;
      throw;
    }
  }

  private async Task SchedulePing()
  {
    await SendStatus(RepeaterStatus.Connected).ConfigureAwait(false);
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

    EnsureRegistrationStatus(res);
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
      // TODO: dispose an event bus
    }
    catch
    {
      // noop
    }
  }

  private void EnsureRegistrationStatus(RegisterRepeaterResult result)
  {
    if (result.Error != RepeaterRegisteringError.None)
    {
      HandleRegisterError(result.Error);
    }
    else
    {
      if (new Version(result.Version!).CompareTo(_version) != 0)
      {
        // TODO: colorize an output in the same manner like sectester-js does
        _logger.LogWarning(
          "(!) IMPORTANT: A new Repeater version ({Version}) is available, please update SecTester.",
          result.Version
        );
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
        "(!) CRITICAL: The current running version is no longer supported, please update SecTester."),
      _ => new ArgumentOutOfRangeException(nameof(error), error, "Something went wrong. Unknown error.")
    };
  }
}
