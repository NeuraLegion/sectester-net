using System;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Core.Internal;

namespace SecTester.Core.Extensions;

public static class SemaphoreSlimExtensions
{
  public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken = default)
  {
    if (semaphore == null)
    {
      throw new ArgumentNullException(nameof(semaphore));
    }

    var releaser = new AutoReleasableSemaphore(semaphore);

    await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

    return releaser;
  }

  public static IDisposable Lock(this SemaphoreSlim semaphore, CancellationToken cancellationToken = default)
  {
    if (semaphore == null)
    {
      throw new ArgumentNullException(nameof(semaphore));
    }

    var releaser = new AutoReleasableSemaphore(semaphore);

    semaphore.Wait(cancellationToken);

    return releaser;
  }
}
