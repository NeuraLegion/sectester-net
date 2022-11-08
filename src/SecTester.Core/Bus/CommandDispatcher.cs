using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public interface CommandDispatcher
{
  Task<TResult?> Execute<TResult>(Command<TResult> message);
  Task<object> Execute(object payload);
}
