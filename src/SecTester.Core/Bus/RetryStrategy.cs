using System;
using System.Threading;
using System.Threading.Tasks;

namespace SecTester.Core.Bus;

public interface RetryStrategy
{
  Task<TResult> Acquire<TResult>(Func<Task<TResult>> task, CancellationToken cancellationToken = default);
}
