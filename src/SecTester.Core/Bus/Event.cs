using System;
using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public class Event<T> : Message<T>
{
  public Event(T payload, string? type, string? correlationId, DateTime? createdAt) : base(payload, type, correlationId, createdAt)
  {
  }
}
