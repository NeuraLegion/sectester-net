using System;

namespace SecTester.Bus.Dispatchers;

internal sealed record MessageParams<T>
{
  public T? Payload { get; init; }
  public string? RoutingKey { get; init; }
  public string? Exchange { get; init; }
  public string? Type { get; init; }
  public string? CorrelationId { get; init; }
  public string? ReplyTo { get; init; }
  public DateTime? CreatedAt { get; init; }
}
