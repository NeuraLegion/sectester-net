using System;
using System.Threading;
using System.Threading.Tasks;
using SocketIOClient;

namespace SecTester.Repeater.Bus;

internal class SocketIoMessage : ISocketIoMessage
{
  private readonly SocketIOResponse _response;

  public SocketIoMessage(SocketIOResponse response)
  {
    _response = response ?? throw new ArgumentNullException(nameof(response));
  }

  public virtual T GetValue<T>(int index = 0) => _response.GetValue<T>(index);

  public virtual Task CallbackAsync(params object[] data) => _response.CallbackAsync(data);

  public virtual Task CallbackAsync(CancellationToken cancellationToken, params object[] data) => _response.CallbackAsync(cancellationToken, data);
}
