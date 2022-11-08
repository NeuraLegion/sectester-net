using System;
using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public class Command<T, TR> : Message<T>
{
  public bool ExpectReply { get; protected set; }
  public int Ttl { get; protected set; }

  public Command(T payload, string? type = null, string? correlationId = null, DateTime? createdAt = null, bool? expectReply = null,
    int? ttl = null) : base(payload, type, correlationId, createdAt)
  {
    ExpectReply = expectReply ?? true;
    Ttl = ttl != null && ttl > 0 ? (int)ttl : 10000;
  }

  public Task<TR?> Execute(CommandDispatcher dispatcher)
  {
    return dispatcher.Execute(this);
  }
}
