using System;
using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public class Command<T, TR> : Message<T>
{
  public bool ExpectReply { get; protected set; }
  public int Ttl { get; protected set; }

  public Command(T payload, string? type, string? correlationId, DateTime? createdAt, bool? expectReply,
    int? ttl) : base(payload, type, correlationId, createdAt)
  {
    ExpectReply = expectReply ?? true;
    Ttl = ttl != null && ttl > 0 ? (int)ttl : 10000;
  }
}
