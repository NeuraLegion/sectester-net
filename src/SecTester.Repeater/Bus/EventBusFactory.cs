using System.Threading.Tasks;
using SecTester.Core.Bus;

namespace SecTester.Repeater.Bus;

public interface EventBusFactory
{
  Task<EventBus> Create(string repeaterId);
}
