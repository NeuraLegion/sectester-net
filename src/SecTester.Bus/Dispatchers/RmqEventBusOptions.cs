using System;

namespace SecTester.Bus.Dispatchers;

public record RmqEventBusOptions(string Url, string AppQueue, string Exchange, string ClientQueue)
{
  public string Url { get; init; } = Url ?? throw new ArgumentNullException(nameof(Url));
  public string AppQueue { get; init; } = AppQueue ?? throw new ArgumentNullException(nameof(AppQueue));
  public string Exchange { get; init; } = Exchange ?? throw new ArgumentNullException(nameof(Exchange));
  public string ClientQueue { get; init; } = ClientQueue ?? throw new ArgumentNullException(nameof(ClientQueue));
  public TimeSpan? ConnectTimeout { get; init; }
  public TimeSpan? HeartbeatInterval { get; init; }
  public string? Password { get; init; }
  public ushort? PrefetchCount { get; init; }
  public TimeSpan? ReconnectTime { get; init; }
  public string? Username { get; init; }
}
