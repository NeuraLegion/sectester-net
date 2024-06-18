using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using SecTester.Core.Utils;

namespace SecTester.Repeater.Bus;

internal sealed class SocketIoRepeaterBus : IRepeaterBus
{
  private static readonly TimeSpan PingInterval = TimeSpan.FromSeconds(10);

  private readonly ITimerProvider _heartbeat;
  private readonly ISocketIoConnection _connection;
  private readonly ILogger<IRepeaterBus> _logger;
  private readonly SocketIoRepeaterBusOptions _options;

  internal SocketIoRepeaterBus(SocketIoRepeaterBusOptions options, ISocketIoConnection connection, ITimerProvider heartbeat, ILogger<IRepeaterBus> logger)
  {
    _options = options ?? throw new ArgumentNullException(nameof(options));
    _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    _heartbeat = heartbeat ?? throw new ArgumentNullException(nameof(heartbeat));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public event Func<IncomingRequest, Task<OutgoingResponse>>? RequestReceived;
  public event Action<Exception>? ErrorOccurred;
  public event Action<Version>? UpgradeAvailable;

  public async Task Connect()
  {
    if (_connection is not { Connected: true })
    {
      DelegateEvents();

      await _connection.Connect().ConfigureAwait(false);

      await SchedulePing().ConfigureAwait(false);

      _logger.LogDebug("Repeater connected to {BaseUrl}", _options.BaseUrl);
    }
  }

  private void DelegateEvents()
  {
    _connection.On("error", response =>
    {
      var err = response.GetValue<RepeaterError>();
      ErrorOccurred?.Invoke(new(err.Message));
    });

    _connection.On("update-available", response =>
    {
      var version = response.GetValue<RepeaterVersion>();
      UpgradeAvailable?.Invoke(new(version.Version));
    });

    _connection.On("request", async response =>
    {
      if (RequestReceived == null)
      {
        return;
      }

      var ct = new CancellationTokenSource(_options.AckTimeout);
      var request = IncomingRequest.FromDictionary(response.GetValue<Dictionary<object, object>>());
      var result = await RequestReceived.Invoke(request).ConfigureAwait(false);
      await response.CallbackAsync(ct.Token, result).ConfigureAwait(false);
    });
  }

  public async ValueTask DisposeAsync()
  {
    if (_connection is { Connected: true })
    {
      _heartbeat.Elapsed -= Ping;
      _heartbeat.Stop();
      await _connection.Disconnect().ConfigureAwait(false);
      _logger.LogDebug("Repeater disconnected from {BaseUrl}", _options.BaseUrl);
    }

    _connection.Dispose();

    RequestReceived = null;
    ErrorOccurred = null;
    UpgradeAvailable = null;

    GC.SuppressFinalize(this);
  }

  public async Task<string> Deploy(CancellationToken? cancellationToken = null)
  {
    try
    {
      var tcs = new TaskCompletionSource<RepeaterInfo>();

      _connection.On("deployed", response => tcs.TrySetResult(response.GetValue<RepeaterInfo>()));

      await _connection
        .EmitAsync("deploy", Array.Empty<object>())
        .ConfigureAwait(false);

      using var _ = cancellationToken?.Register(() => tcs.TrySetCanceled());

      var result = await tcs.Task.ConfigureAwait(false);

      _logger.LogDebug("Repeater ({RepeaterId}) deployed", result?.RepeaterId);

      if (null == result || string.IsNullOrWhiteSpace(result.RepeaterId))
      {
        throw new InvalidOperationException("An error occured while repeater is being deployed");
      }

      return result.RepeaterId;
    }
    finally
    {
      _connection.Off("deployed");
    }
  }

  private async Task SchedulePing()
  {
    await Ping().ConfigureAwait(false);
    _heartbeat.Interval = PingInterval.TotalMilliseconds;
    _heartbeat.Elapsed += Ping;
    _heartbeat.Start();
  }

  private async void Ping(object sender, ElapsedEventArgs args)
  {
    await Ping().ConfigureAwait(false);
  }

  private async Task Ping()
  {
    await _connection.EmitAsync("ping").ConfigureAwait(false);
  }
}
