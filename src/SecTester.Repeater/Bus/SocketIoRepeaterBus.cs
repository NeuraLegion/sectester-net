using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using SecTester.Core.Utils;

namespace SecTester.Repeater.Bus;

public class SocketIoRepeaterBus : IRepeaterBus
{
  private static readonly TimeSpan PingInterval = TimeSpan.FromSeconds(10);

  private readonly ITimerProvider _heartbeat;
  private readonly ISocketIoClient _client;
  private readonly ILogger<IRepeaterBus> _logger;
  private readonly Uri _url;

  public SocketIoRepeaterBus(Uri url, ISocketIoClient client, ITimerProvider heartbeat, ILogger<IRepeaterBus> logger)
  {
    _url = url ?? throw new ArgumentNullException(nameof(url));
    _client = client ?? throw new ArgumentNullException(nameof(client));
    _heartbeat = heartbeat ?? throw new ArgumentNullException(nameof(heartbeat));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  internal record RepeaterVersion(string Version);
  internal record RepeaterError(string Message);
  internal record RepeaterInfo(string RepeaterId);

  public event Func<IncomingRequest, Task<OutgoingResponse>>? RequestReceived;
  public event Action<Exception>? ErrorOccurred;
  public event Action<Version>? UpgradeAvailable;

  public async Task Connect()
  {
    if (_client is not { Connected: true })
    {
      DelegateEvents();

      await _client.Connect().ConfigureAwait(false);

      await SchedulePing().ConfigureAwait(false);

      _logger.LogDebug("Repeater connected to {Url}", _url);
    }
  }

  private void DelegateEvents()
  {
    _client.On("error", response =>
    {
      var err = response.GetValue<RepeaterError>();
      ErrorOccurred?.Invoke(new(err.Message));
    });

    _client.On("update-available", response =>
    {
      var version = response.GetValue<RepeaterVersion>();
      UpgradeAvailable?.Invoke(new(version.Version));
    });

    _client.On("request", async response =>
    {
      if (RequestReceived == null)
      {
        return;
      }

      var request = response.GetValue<IncomingRequest>();
      var result = await RequestReceived.Invoke(request).ConfigureAwait(false);
      await response.CallbackAsync(result).ConfigureAwait(false);
    });
  }

  public async ValueTask DisposeAsync()
  {
    if (_client is { Connected: true })
    {
      _heartbeat.Elapsed -= Ping;
      _heartbeat.Stop();
      await _client.Disconnect().ConfigureAwait(false);
      _logger.LogDebug("Repeater disconnected from {Url}", _url);
    }

    _client.Dispose();

    RequestReceived = null;
    ErrorOccurred = null;
    UpgradeAvailable = null;

    GC.SuppressFinalize(this);
  }

  public async Task Deploy(string repeaterId, Runtime? runtime = null, CancellationToken? cancellationToken = null)
  {
    try
    {
      var tcs = new TaskCompletionSource<RepeaterInfo>();

      _client.On("deployed", response => tcs.TrySetResult(response.GetValue<RepeaterInfo>()));

      await _client.EmitAsync("deploy", new RepeaterInfo(repeaterId), runtime).ConfigureAwait(false);

      using var _ = cancellationToken?.Register(() => tcs.TrySetCanceled());

      var result = await tcs.Task.ConfigureAwait(false);

      _logger.LogDebug("Repeater ({RepeaterId}) deployed", result?.RepeaterId);
    }
    finally
    {
      _client.Off("deployed");
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
    await _client.EmitAsync("ping").ConfigureAwait(false);
  }
}
