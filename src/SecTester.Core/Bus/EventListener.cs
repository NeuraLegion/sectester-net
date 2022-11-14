using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public interface EventListener<in TEvent, TResult> where TEvent : Event
{
  Task<TResult> Handle(TEvent message);
}

public interface EventListener<in TEvent> : EventListener<TEvent, Unit> where TEvent : Event
{ }
