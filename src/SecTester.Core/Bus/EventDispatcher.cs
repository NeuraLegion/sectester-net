using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public interface EventDispatcher
{
  Task Publish(Event message);
}
