using System;
using System.Threading.Tasks;

namespace SecTester.Repeater.Bus;

internal interface ISocketIoConnection : IDisposable
{
  public bool Connected { get; }
  public Task Connect();
  public Task Disconnect();
  public void On(string eventName, Action<ISocketIoMessage> callback);
  public void Off(string eventName);
  public Task EmitAsync(string eventName, params object[] data);
}
