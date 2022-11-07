using System;

namespace SecTester.Core.Bus;

public abstract class Message<T>
{
  public string CorrelationId { get; protected set; }
  public DateTime CreatedAt { get; protected set; }
  public T Payload { get; protected set; }
  public string Type { get; protected set; }

  protected Message(
    T payload,
    string? type,
    string? correlationId,
    DateTime? createdAt
  )
  {
    Payload = payload;
    Type = type ?? this.GetType().Name;
    CorrelationId = correlationId ?? Guid.NewGuid().ToString();
    CreatedAt = createdAt ?? DateTime.Now;
  }
}
