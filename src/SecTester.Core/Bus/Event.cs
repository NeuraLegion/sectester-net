using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public record Event : Message
{
  public Task Publish(EventDispatcher dispatcher)
  {
    return dispatcher.Publish(this);
  }
}
