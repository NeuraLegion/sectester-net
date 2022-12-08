using System.Collections.Generic;
using System.Threading.Tasks;
using SecTester.Core;
using SecTester.Core.Bus;
using SecTester.Core.Exceptions;
using SecTester.Scan.CI;
using SecTester.Scan.Commands;
using SecTester.Scan.Models;

namespace SecTester.Scan;

public class DefaultScans : Scans
{
  private readonly CiDiscovery _ciDiscovery;
  private readonly CommandDispatcher _commandDispatcher;
  private readonly Configuration _configuration;

  public DefaultScans(Configuration configuration, CommandDispatcher commandDispatcher, CiDiscovery ciDiscovery)
  {
    _configuration = configuration;
    _commandDispatcher = commandDispatcher;
    _ciDiscovery = ciDiscovery;
  }

  public async Task<string> CreateScan(ScanConfig config)
  {
    var command = new CreateScan(config, _configuration.Name, _configuration.Version, _ciDiscovery.Server?.ServerName);

    var result = await SendCommand(command).ConfigureAwait(false);

    return result.Id;
  }

  public Task<IEnumerable<Issue>> ListIssues(string id)
  {
    return SendCommand(new ListIssues(id));
  }

  public async Task StopScan(string id)
  {
    await _commandDispatcher.Execute(new StopScan(id)).ConfigureAwait(false);
  }

  public async Task DeleteScan(string id)
  {
    await _commandDispatcher.Execute(new DeleteScan(id)).ConfigureAwait(false);
  }

  public Task<ScanState> GetScan(string id)
  {
    return SendCommand(new GetScan(id));
  }

  public async Task<string> UploadHar(UploadHarOptions options)
  {
    var result = await SendCommand(new UploadHar(options)).ConfigureAwait(false);

    return result.Id;
  }

  private async Task<T> SendCommand<T>(Command<T> command)
  {
    var result = await _commandDispatcher.Execute(command).ConfigureAwait(false);

    return AssertReply(result);
  }

  private static T AssertReply<T>(T? reply)
  {
    if (reply is null)
    {
      throw new SecTesterException("Something went wrong. Please try again later.");
    }

    return reply;
  }
}
