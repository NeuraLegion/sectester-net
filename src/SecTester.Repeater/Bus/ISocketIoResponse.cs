using System.Threading;
using System.Threading.Tasks;

namespace SecTester.Repeater.Bus;

public interface ISocketIoResponse
{
  public T GetValue<T>(int i = 0);
  public Task CallbackAsync(params object[] data);
  public Task CallbackAsync(CancellationToken cancellationToken, params object[] data);
}
