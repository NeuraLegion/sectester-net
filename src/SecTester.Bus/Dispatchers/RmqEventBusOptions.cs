using System;

namespace SecTester.Bus.Dispatchers;

public sealed record RmqEventBusOptions(string Url, string AppQueue, string Exchange, string ClientQueue)
{
  public TimeSpan ConnectTimeout { get; init; } = TimeSpan.FromSeconds(30);
  public TimeSpan HeartbeatInterval { get; init; } = TimeSpan.FromSeconds(30);
  public ushort PrefetchCount { get; init; } = 1;
  public TimeSpan ReconnectTime { get; init; } = TimeSpan.FromSeconds(30);
  public string? Password { get; init; }
  public string? Username { get; init; }
}
