using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public interface EventDispatcher
{
  Task Publish<TEvent>(TEvent message) where TEvent : Event;
}
