using System;

namespace SecTester.Bus.Dispatchers;

public record RMQEventBusOptions
{
  public readonly string? AppQueue;
  public readonly string? ClientQueue;
  public readonly TimeSpan? ConnectTimeout;
  public readonly string? Exchange;
  public readonly TimeSpan? HeartbeatInterval;
  public readonly string? Password;
  public readonly int? PrefetchCount;
  public readonly TimeSpan? ReconnectTime;
  public readonly string? Url;
  public readonly string? Username;
}
