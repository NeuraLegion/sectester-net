using System.Threading.Tasks;
using SecTester.Core.Bus;
using SecTester.Core.Exceptions;

namespace SecTester.Repeater.Api;

public class DefaultRepeaters : IRepeaters
{
  private readonly ICommandDispatcher _commandDispatcher;

  public DefaultRepeaters(ICommandDispatcher commandDispatcher)
  {
    _commandDispatcher = commandDispatcher;
  }

  public async Task<string> CreateRepeater(string name, string? description = default)
  {
    var repeaterId = (await _commandDispatcher.Execute(new CreateRepeaterRequest(name, description)).ConfigureAwait(false))?.Id;

    if (string.IsNullOrEmpty(repeaterId))
    {
      throw new SecTesterException("Cannot create repeater");
    }

    return repeaterId!;
  }

  public async Task DeleteRepeater(string repeaterId)
  {
    await _commandDispatcher.Execute(
      new DeleteRepeaterRequest(repeaterId)
    ).ConfigureAwait(false);
  }
}
