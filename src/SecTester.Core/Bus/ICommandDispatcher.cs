using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public interface ICommandDispatcher
{
  Task<TResult?> Execute<TResult>(Command<TResult> message);
}
