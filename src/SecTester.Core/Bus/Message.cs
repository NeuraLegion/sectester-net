using System;

namespace SecTester.Core.Bus;

public abstract record Message
{
  public string CorrelationId { get; protected init; }
  public DateTime CreatedAt { get; protected init; }
  public string Type { get; protected init; }

  protected Message()
  {
    Type = this.GetType().Name;
    CorrelationId = Guid.NewGuid().ToString();
    CreatedAt = DateTime.Now;
  }

  protected Message(
    string type,
    string correlationId,
    DateTime createdAt
  )
  {
    Type = type;
    CorrelationId = correlationId;
    CreatedAt = createdAt;
  }
}
