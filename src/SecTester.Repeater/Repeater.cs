using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SecTester.Core;
using SecTester.Core.Bus;
using SecTester.Core.Exceptions;
using SecTester.Repeater.Bus;
using Timer = System.Timers.Timer;

namespace SecTester.Repeater;

public class Repeater : IAsyncDisposable
{
  private static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromSeconds(10);
  private readonly Configuration _configuration;
  private readonly EventBus _eventBus;
  private readonly ILogger _logger;
  private readonly SemaphoreSlim _semaphore = new(1, 1);
  private Timer? _heartbeat;

  public Repeater(string repeaterId, EventBus eventBus, Configuration configuration, ILogger logger)
  {
    RepeaterId = repeaterId;
    _logger = logger;
    _configuration = configuration;
    _eventBus = eventBus;
  }

  public RunningStatus Status { get; private set; } = RunningStatus.Off;
  public string RepeaterId { get; }

  public async ValueTask DisposeAsync()
  {
    await Stop().ConfigureAwait(false);
    GC.SuppressFinalize(this);
  }

  public async Task Start()
  {
    if (Status != RunningStatus.Off)
    {
      throw new InvalidOperationException("Repeater is already active.");
    }

    try
    {
      await _semaphore.WaitAsync().ConfigureAwait(false);

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
    finally
    {
      _semaphore.Release();
    }
  }

  private async Task SchedulePing()
  {
    await SendStatus(RepeaterStatus.Connected).ConfigureAwait(false);
    _heartbeat = new Timer
    {
      Enabled = true,
      Interval = DefaultPollingInterval.TotalMilliseconds
    };
    _heartbeat.Elapsed += async (_, _) => await SendStatus(RepeaterStatus.Connected).ConfigureAwait(false);
  }

  private async Task SendStatus(RepeaterStatus status)
  {
    var @event = new RepeaterStatusEvent(RepeaterId, status);
    await _eventBus.Publish(@event).ConfigureAwait(false);
  }

  private void SubscribeToEvents()
  {
    throw new NotImplementedException();
  }

  private async Task Register()
  {
    var command = new RegisterRepeaterCommand(_configuration.RepeaterVersion, RepeaterId);
    var res = await _eventBus.Execute(command).ConfigureAwait(false);

    if (res == null)
    {
      throw new SecTesterException("Error registering repeater.");
    }

    EnsureRegistrationStatus(res);
  }

  public async Task Stop()
  {
    try
    {
      await _semaphore.WaitAsync().ConfigureAwait(false);
      if (Status != RunningStatus.Running)
      {
        return;
      }

      Status = RunningStatus.Off;
      _heartbeat?.Dispose();
      await SendStatus(RepeaterStatus.Disconnected).ConfigureAwait(false);
      // TODO: dispose an event bus
    }
    finally
    {
      _semaphore.Release();
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
      if (new Version(result.Version!).CompareTo(new Version(_configuration.RepeaterVersion)) != 0)
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
