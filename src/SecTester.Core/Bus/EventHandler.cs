using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public interface EventHandler<in T, TR>
{
  Task<TR> Handle(T payload);
}

public interface EventHandler<in T> : EventHandler<T, Unit>
{ }
