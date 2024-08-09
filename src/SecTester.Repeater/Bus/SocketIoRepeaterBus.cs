using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SecTester.Repeater.Bus;

internal sealed class SocketIoRepeaterBus : IRepeaterBus
{
  private static readonly TimeSpan PingInterval = TimeSpan.FromSeconds(10);

  private readonly ISocketIoConnection _connection;
  private readonly ILogger<IRepeaterBus> _logger;
  private readonly SocketIoRepeaterBusOptions _options;

  internal SocketIoRepeaterBus(SocketIoRepeaterBusOptions options, ISocketIoConnection connection, ILogger<IRepeaterBus> logger)
  {
    _options = options ?? throw new ArgumentNullException(nameof(options));
    _connection = connection ?? throw new ArgumentNullException(nameof(connection));
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
      await _connection.Disconnect().ConfigureAwait(false);
      _logger.LogDebug("Repeater disconnected from {BaseUrl}", _options.BaseUrl);
    }

    _connection.Dispose();

    RequestReceived = null;
    ErrorOccurred = null;
    UpgradeAvailable = null;

    GC.SuppressFinalize(this);
  }

  public async Task Deploy(string repeaterId, CancellationToken? cancellationToken = null)
  {
    try
    {
      var tcs = new TaskCompletionSource<RepeaterInfo>();

      _connection.On("deployed", response => tcs.TrySetResult(response.GetValue<RepeaterInfo>()));

      await _connection.EmitAsync("deploy", new RepeaterInfo { RepeaterId = repeaterId }).ConfigureAwait(false);

      using var _ = cancellationToken?.Register(() => tcs.TrySetCanceled());

      var result = await tcs.Task.ConfigureAwait(false);

      _logger.LogDebug("Repeater ({RepeaterId}) deployed", result?.RepeaterId);
    }
    finally
    {
      _connection.Off("deployed");
    }
  }
}
