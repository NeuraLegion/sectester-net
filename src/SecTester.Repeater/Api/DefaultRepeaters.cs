using System;
using System.Linq;
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
    await _commandDispatcher.Execute(new CreateRepeaterRequest(name, description)).ConfigureAwait(false);

    var repeaterId = (await FindRepeaterByName(name).ConfigureAwait(false))?.Id;

    if (string.IsNullOrEmpty(repeaterId))
    {
      throw new SecTesterException("Cannot find created repeater id");
    }

    return repeaterId!;
  }

  public async Task DeleteRepeater(string repeaterId)
  {
    await _commandDispatcher.Execute(
      new DeleteRepeaterRequest(repeaterId)
    ).ConfigureAwait(false);
  }

  private async Task<RepeaterIdentity?> FindRepeaterByName(string name)
  {
    var repeaters = await _commandDispatcher.Execute(new ListRepeatersRequest()).ConfigureAwait(false);
    return repeaters?.FirstOrDefault(repeater => repeater.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
  }
}
