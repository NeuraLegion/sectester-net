using System;
using System.Threading;
using System.Threading.Tasks;

namespace SecTester.Repeater.Bus;

public interface IRepeaterBus : IAsyncDisposable
{
  event Func<IncomingRequest, Task<OutgoingResponse>> RequestReceived;
  event Action<Exception> ErrorOccurred;
  event Action<Version> UpgradeAvailable;

  Task Connect();
  Task Deploy(string repeaterId, CancellationToken? cancellationToken = null);
}
