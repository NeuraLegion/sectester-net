using System;
using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public record Command<TResponse> : Message
{
  private readonly TimeSpan _ttl;
  public bool ExpectReply { get; protected init; }

  public TimeSpan Ttl
  {
    get => _ttl;
    protected init
    {
      if (value <= TimeSpan.Zero)
      {
        throw new ArgumentException($"{nameof(Ttl)} must be greater than 0.");
      }

      _ttl = value;
    }
  }

  public Command(bool? expectReply = null, TimeSpan? ttl = null)
  {
    ExpectReply = expectReply ?? true;
    Ttl = ttl ?? TimeSpan.FromMinutes(1);
  }

  public Command(string type, string correlationId, DateTime createdAt, bool expectReply, TimeSpan ttl) : base(type, correlationId, createdAt)
  {
    ExpectReply = expectReply;
    Ttl = ttl;
  }

  public Task<TResponse?> Execute(ICommandDispatcher dispatcher)
  {
    return dispatcher.Execute(this);
  }
}
