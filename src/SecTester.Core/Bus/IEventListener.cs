using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public interface IEventListener<in TEvent, TResult> where TEvent : Event
{
  Task<TResult> Handle(TEvent message);
}

public interface IEventListener<in TEvent> : IEventListener<TEvent, Unit> where TEvent : Event
{ }
