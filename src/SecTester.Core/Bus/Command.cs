using System;
using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public record Command<TResponse> : Message
{
  private readonly int _ttl;
  private const int DefaultTtl = 10000;
  public bool ExpectReply { get; protected init; }

  public int Ttl
  {
    get => _ttl;
    protected init => _ttl = value > 0 ? value : DefaultTtl;
  }

  public Command()
  {
    ExpectReply = true;
    Ttl = DefaultTtl;
  }

  public Command(bool expectReply, int ttl)
  {
    ExpectReply = expectReply;
    Ttl = ttl;
  }

  public Command(string type, string correlationId, DateTime createdAt, bool expectReply, int ttl) : base(type, correlationId, createdAt)
  {
    ExpectReply = expectReply;
    Ttl = ttl;
  }

  public Task<TResponse?> Execute(CommandDispatcher dispatcher)
  {
    return dispatcher.Execute(this);
  }
}
