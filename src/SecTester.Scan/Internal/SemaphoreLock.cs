using System;
using System.Threading;
using System.Threading.Tasks;

namespace SecTester.Scan.Internal;

internal class SemaphoreLock : IDisposable
{
  private readonly SemaphoreSlim _semaphore;

  private SemaphoreLock(SemaphoreSlim semaphore)
  {
    _semaphore = semaphore ?? throw new ArgumentNullException(nameof(semaphore));
  }

  public static async Task<IDisposable> LockAsync(SemaphoreSlim semaphore, CancellationToken cancellationToken = default)
  {
    var releaser = new SemaphoreLock(semaphore);

    await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

    return releaser;
  }

  public static IDisposable Lock(SemaphoreSlim semaphore)
  {
    var releaser = new SemaphoreLock(semaphore);

    semaphore.Wait();

    return releaser;
  }

  public void Dispose()
  {
    _semaphore.Release();
    GC.SuppressFinalize(this);
  }
}
