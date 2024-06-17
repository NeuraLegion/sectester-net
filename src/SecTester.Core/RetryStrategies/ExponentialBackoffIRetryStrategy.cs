using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Core.Bus;
using SecTester.Core.Exceptions;

namespace SecTester.Core.RetryStrategies;

public class ExponentialBackoffIRetryStrategy : IRetryStrategy
{
  private readonly ExponentialBackoffOptions _options;

  public ExponentialBackoffIRetryStrategy(ExponentialBackoffOptions options)
  {
    _options = options;
  }

  public async Task<TResult> Acquire<TResult>(Func<Task<TResult>> task, CancellationToken cancellationToken = default)
  {
    var depth = 0;

    for ( ; ; )
    {
      try
      {
        cancellationToken.ThrowIfCancellationRequested();

        return await task().ConfigureAwait(false);
      }
      catch (Exception e)
      {
        depth++;

        if (!ShouldRetry(e) || depth > _options.MaxDepth)
        {
          throw;
        }

        var retryAttempt = TimeSpan.FromMilliseconds(Math.Pow(2, depth) * _options.MinInterval);
        await Task.Delay(retryAttempt, cancellationToken).ConfigureAwait(false);
      }
    }
  }

  private static bool ShouldRetry(Exception err)
  {
    return err is SocketException or TaskCanceledException or HttpStatusException { Retryable: true };
  }
}
