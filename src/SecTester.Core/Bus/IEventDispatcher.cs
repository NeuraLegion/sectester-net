using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public interface IEventDispatcher
{
  Task Publish<TEvent>(TEvent message) where TEvent : Event;
}
