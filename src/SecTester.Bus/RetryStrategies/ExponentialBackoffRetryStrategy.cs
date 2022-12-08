using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.Exceptions;
using SecTester.Core.Bus;

namespace SecTester.Bus.RetryStrategies;

public class ExponentialBackoffRetryStrategy : RetryStrategy
{
  private readonly ExponentialBackoffOptions _options;

  public ExponentialBackoffRetryStrategy(ExponentialBackoffOptions options)
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
    return err is SocketException or BrokerUnreachableException or TaskCanceledException or HttpRequestException;
  }
}
