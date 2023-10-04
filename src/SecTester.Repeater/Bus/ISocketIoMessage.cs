using System.Threading;
using System.Threading.Tasks;

namespace SecTester.Repeater.Bus;

internal interface ISocketIoMessage
{
  public T GetValue<T>(int index = 0);
  public Task CallbackAsync(params object[] data);
  public Task CallbackAsync(CancellationToken cancellationToken, params object[] data);
}
