namespace SecTester.Repeater.Tests.Extensions;

public class SemaphoreSlimExtensionsTests
{
  [Fact]
  public async Task LockAsync_Nullable_ThrowsException()
  {
    // arrange
    var semaphoreSlim = null as SemaphoreSlim;

    // act
    var act = () => semaphoreSlim!.LockAsync();

    // assert
    await act.Should().ThrowAsync<ArgumentNullException>();
  }

  [Fact]
  public async Task LockAsync_OperationAborted_ThrowsError()
  {
    // arrange
    var semaphoreSlim = new SemaphoreSlim(1, 1);
    using var cts = new CancellationTokenSource();
    cts.Cancel();

    // act
    var act = () => semaphoreSlim.LockAsync(cts.Token);

    // assert
    await act.Should().ThrowAsync<OperationCanceledException>();
  }

  [Fact]
  public async Task LockAsync_ReturnsDisposable()
  {
    // arrange
    var semaphoreSlim = new SemaphoreSlim(1, 1);

    // act
    using var releaser = await semaphoreSlim.LockAsync();

    // assert
    releaser.Should().BeAssignableTo<IDisposable>();
  }

  [Fact]
  public async Task LockAsync_Disposed_ReleasesLock()
  {
    // arrange
    var semaphoreSlim = new SemaphoreSlim(1, 1);

    // act
    using (var _ = await semaphoreSlim.LockAsync())
    {
      semaphoreSlim.CurrentCount.Should().Be(0);
    }

    // assert
    semaphoreSlim.CurrentCount.Should().Be(1);
  }

  [Fact]
  public async Task LockAsync_SemaphoreFullException_HandlesException()
  {
    // arrange
    var semaphoreSlim = new SemaphoreSlim(1, 1);

    // act
    var act = async () =>
    {
      using var _ = await semaphoreSlim.LockAsync();
      semaphoreSlim.Release();
    };

    // assert
    await act.Should().NotThrowAsync<SemaphoreFullException>();
  }

  [Fact]
  public void Lock_ReturnsDisposable()
  {
    // arrange
    var semaphoreSlim = new SemaphoreSlim(1, 1);

    // act
    using var releaser = semaphoreSlim.Lock();

    // assert
    releaser.Should().BeAssignableTo<IDisposable>();
  }

  [Fact]
  public void Lock_Disposed_ReleasesLock()
  {
    // arrange
    var semaphoreSlim = new SemaphoreSlim(1, 1);

    // act
    using (var _ = semaphoreSlim.Lock())
    {
      semaphoreSlim.CurrentCount.Should().Be(0);
    }

    // assert
    semaphoreSlim.CurrentCount.Should().Be(1);
  }

  [Fact]
  public void Lock_SemaphoreFullException_HandlesException()
  {
    // arrange
    var semaphoreSlim = new SemaphoreSlim(1, 1);

    // act
    var act = () =>
    {
      using var _ = semaphoreSlim.Lock();
      semaphoreSlim.Release();
    };

    // assert
    act.Should().NotThrow<SemaphoreFullException>();
  }

  [Fact]
  public void Lock_OperationAborted_ThrowsError()
  {
    // arrange
    var semaphoreSlim = new SemaphoreSlim(1, 1);
    using var cts = new CancellationTokenSource();
    cts.Cancel();

    // act
    var act = () => semaphoreSlim.Lock(cts.Token);

    // assert
    act.Should().Throw<OperationCanceledException>();
  }

  [Fact]
  public void Lock_Nullable_ThrowsException()
  {
    // arrange
    var semaphoreSlim = null as SemaphoreSlim;

    // act
    var act = () => semaphoreSlim!.Lock();

    // assert
    act.Should().Throw<ArgumentNullException>();
  }
}
