using System;
using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public class Event<T> : Message<T>
{
  public Event(T payload, string? type = null, string? correlationId = null, DateTime? createdAt = null) : base(payload, type, correlationId, createdAt)
  {
  }

  public Task Publish(EventDispatcher dispatcher)
  {
    return dispatcher.Publish(this);
  }
}
