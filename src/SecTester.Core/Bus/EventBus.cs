namespace SecTester.Core.Bus;

public interface EventBus : EventDispatcher, CommandDispatcher
{
  void Register<THandler, TEvent>()
    where THandler : EventListener<TEvent>
    where TEvent : Event;

  void Register<THandler, TEvent, TResult>()
    where THandler : EventListener<TEvent, TResult>
    where TEvent : Event;

  void Unregister<THandler, TEvent>()
    where THandler : EventListener<TEvent>
    where TEvent : Event;

  void Unregister<THandler, TEvent, TResult>()
    where THandler : EventListener<TEvent, TResult>
    where TEvent : Event;
}
