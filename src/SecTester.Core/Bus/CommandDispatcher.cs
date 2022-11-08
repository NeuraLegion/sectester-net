using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public interface CommandDispatcher
{
  Task<TR?> Execute<T, TR>(Command<T, TR> message);
  Task<object> Execute(object payload);
}
