using System;
using System.Threading.Tasks;

namespace SecTester.Repeater.Bus;

internal sealed class SocketIoConnection : ISocketIoConnection
{
  private readonly SocketIOClient.SocketIO _socketIo;

  public SocketIoConnection(SocketIOClient.SocketIO socketIo) => _socketIo = socketIo ?? throw new ArgumentNullException(nameof(socketIo));

  public void Dispose()
  {
    _socketIo.Dispose();
    GC.SuppressFinalize(this);
  }

  public bool Connected => _socketIo.Connected;

  public Task Connect() => _socketIo.ConnectAsync();

  public Task Disconnect() => _socketIo.DisconnectAsync();

  public void On(string eventName, Action<ISocketIoMessage> callback) => _socketIo.On(eventName, x => callback(new SocketIoMessage(x)));

  public void Off(string eventName) => _socketIo.Off(eventName);

  public Task EmitAsync(string eventName, params object[] data) => _socketIo.EmitAsync(eventName, data);
}
