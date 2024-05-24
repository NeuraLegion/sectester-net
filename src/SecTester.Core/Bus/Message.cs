using System;
using System.Runtime.Serialization;
using SecTester.Core.Utils;

namespace SecTester.Core.Bus;

public abstract record Message
{
  [IgnoreDataMember]
  public string CorrelationId { get; protected init; }

  [IgnoreDataMember]
  public DateTime CreatedAt { get; protected init; }

  [IgnoreDataMember]
  public string Type { get; protected init; }

  protected Message()
  {
    Type = MessageUtils.GetMessageType(GetType());
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
