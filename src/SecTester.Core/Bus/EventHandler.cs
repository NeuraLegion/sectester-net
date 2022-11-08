using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public interface EventHandler<in T, TR> where T : Event
{
  Task<TR> Handle(T message);
}

public interface EventHandler<in T> : EventHandler<T, Unit> where T : Event
{ }
