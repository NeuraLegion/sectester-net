using System;

namespace SecTester.Core.Bus;

public interface IEventBus : IEventDispatcher, ICommandDispatcher, IDisposable
{
  void Register<THandler, TEvent>()
    where THandler : IEventListener<TEvent>
    where TEvent : Event;

  void Register<THandler, TEvent, TResult>()
    where THandler : IEventListener<TEvent, TResult>
    where TEvent : Event;

  void Unregister<THandler, TEvent>()
    where THandler : IEventListener<TEvent>
    where TEvent : Event;

  void Unregister<THandler, TEvent, TResult>()
    where THandler : IEventListener<TEvent, TResult>
    where TEvent : Event;
}
