using System;
using System.Threading;

namespace SecTester.Core.Internal;

internal class AutoReleasableSemaphore : IDisposable
{
  private readonly SemaphoreSlim _semaphore;

  public AutoReleasableSemaphore(SemaphoreSlim semaphore)
  {
    _semaphore = semaphore ?? throw new ArgumentNullException(nameof(semaphore));
  }

  public void Dispose()
  {
    try
    {
      _semaphore.Release();
    }
    catch
    {
      // noop
    }

    GC.SuppressFinalize(this);
  }
}
