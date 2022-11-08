namespace SecTester.Core.Bus;

public interface EventBus : EventDispatcher, CommandDispatcher
{
  void Register<THandler, TEvent>()
    where THandler : EventHandler<TEvent>
    where TEvent : Event;

  void Unregister<THandler, TEvent>()
    where THandler : EventHandler<TEvent>
    where TEvent : Event;
}
