using System;
using System.Threading;
using System.Threading.Tasks;

namespace SecTester.Repeater;

public interface IRepeater : IAsyncDisposable
{
  string RepeaterId { get; }
  Task Start(CancellationToken cancellationToken = default);
  Task Stop(CancellationToken cancellationToken = default);
}
