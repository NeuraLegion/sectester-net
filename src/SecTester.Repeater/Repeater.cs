using System;
using System.Threading.Tasks;
using SecTester.Core;
using SecTester.Core.Bus;

namespace SecTester.Repeater;

public class Repeater : IAsyncDisposable
{
  private readonly Configuration _configuration;
  private readonly EventBus _eventBus;

  public Repeater(string repeaterId, EventBus eventBus, Configuration configuration)
  {
    RepeaterId = repeaterId;
    _configuration = configuration;
    _eventBus = eventBus;
  }

  public string RepeaterId { get; }

  public async ValueTask DisposeAsync()
  {
    await Stop().ConfigureAwait(false);
    GC.SuppressFinalize(this);
  }

  public Task Start()
  {
    throw new NotImplementedException();
  }

  public Task Stop()
  {
    throw new NotImplementedException();
  }
}
