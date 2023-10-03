using System;
using System.Threading.Tasks;

namespace SecTester.Repeater.Bus;

public class SocketIoClientWrapper : ISocketIoClient
{
  private readonly SocketIOClient.SocketIO _socketIo;

  public SocketIoClientWrapper(SocketIOClient.SocketIO socketIo) => _socketIo = socketIo ?? throw new ArgumentNullException(nameof(socketIo));

  public void Dispose()
  {
    _socketIo.Dispose();
    GC.SuppressFinalize(this);
  }

  public bool Connected => _socketIo.Connected;

  public Task Connect() => _socketIo.ConnectAsync();

  public Task Disconnect() => _socketIo.DisconnectAsync();

  public void On(string eventName, Action<ISocketIoResponse> callback) => _socketIo.On(eventName, x => callback(x as ISocketIoResponse));

  public void Off(string eventName) => _socketIo.Off(eventName);

  public Task EmitAsync(string eventName, params object[] data) => _socketIo.EmitAsync(eventName, data);
}
