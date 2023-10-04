using System;

namespace SecTester.Repeater.Bus;

internal record SocketIoRepeaterBusOptions(Uri BaseUrl)
{
  public Uri Url => new(BaseUrl, "/workstations");
  public string Path { get; init; } = "/api/ws/v1";
  public TimeSpan ConnectionTimeout { get; init; } = TimeSpan.FromSeconds(10);
  public TimeSpan AckTimeout { get; init; } = TimeSpan.FromSeconds(60);
  public int ReconnectionDelayMax { get; init; } = 86_400_000;
  public int ReconnectionAttempts { get; init; } = 20;
}
