using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public record Event : Message
{
  public Task Publish(IEventDispatcher dispatcher)
  {
    return dispatcher.Publish(this);
  }
}
