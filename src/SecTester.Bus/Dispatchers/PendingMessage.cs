using System;
using System.Text;

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

  public string ToJson() => Payload is not null ? MessageSerializer.Serialize(Payload) : "";
  public byte[] ToBytes() => Encoding.UTF8.GetBytes(ToJson());
}

