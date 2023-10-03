using System;
using System.Threading.Tasks;

namespace SecTester.Repeater.Bus;

public interface ISocketIoClient : IDisposable
{
  public bool Connected { get; }
  public Task Connect();
  public Task Disconnect();
  public void On(string eventName, Action<ISocketIoResponse> callback);
  public void Off(string eventName);
  public Task EmitAsync(string eventName, params object[] data);
}
