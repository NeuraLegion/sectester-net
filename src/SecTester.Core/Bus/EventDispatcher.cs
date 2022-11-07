using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public interface EventDispatcher
{
  Task Publish<T>(Event<T> message);
}
