using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SecTester.Bus.Dispatchers;
using SecTester.Bus.Extensions;
using SecTester.Core;
using SecTester.Core.Bus;
using SecTester.Core.Exceptions;
using SecTester.Scan.CI;
using SecTester.Scan.Commands;
using SecTester.Scan.Models;

namespace SecTester.Scan;

internal class DefaultScans : Scans
{
  private readonly Configuration _configuration;
  private readonly CommandDispatcher _commandDispatcher;
  private readonly MessageSerializer _messageSerializer;
  private readonly CiDiscovery _ciDiscovery;

  public DefaultScans(Configuration configuration, CommandDispatcher commandDispatcher,
    MessageSerializer messageSerializer, CiDiscovery ciDiscovery)
  {
    _configuration = configuration;
    _commandDispatcher = commandDispatcher;
    _messageSerializer = messageSerializer;
    _ciDiscovery = ciDiscovery;
  }

  public async Task<string> CreateScan(ScanConfig config)
  {
    var payload = new
    {
      config.Name,
      config.Module,
      config.Tests,
      config.DiscoveryTypes,
      config.PoolSize,
      config.AttackParamLocations,
      config.FileId,
      config.HostsFilter,
      config.Repeaters,
      config.Smart,
      config.SkipStaticParams,
      config.ProjectId,
      config.SlowEpTimeout,
      config.TargetTimeout,
      Info = new
      {
        Source = "utlib", client = new { _configuration.Name, _configuration.Version }, Provider = _ciDiscovery.Server.ServerName
      }
    };

    var content = _messageSerializer.SerializeJsonContent(payload);
    var result = await SendCommand(new CreateScan(content));

    return result.Id;
  }

  public async Task<IEnumerable<Issue>> ListIssues(string id)
  {
    return await SendCommand(new ListIssues(id));
  }

  public async Task StopScan(string id)
  {
    await _commandDispatcher.Execute(new StopScan(id));
  }

  public async Task DeleteScan(string id)
  {
    await _commandDispatcher.Execute(new DeleteScan(id));
  }

  public async Task<ScanState> GetScan(string id)
  {
    return await SendCommand(new GetScan(id));
  }

  public async Task<string> UploadHar(UploadHarOptions options)
  {
    var jsonFileContent = _messageSerializer.SerializeJsonContent(options.Har);
    var fileNameContent = new StringContent(options.FileName);
    
    var requestContent = new MultipartFormDataContent();
    requestContent.Add(jsonFileContent, "file");
    requestContent.Add(fileNameContent, "filename");
    
    var result =  await SendCommand(new UploadHar(requestContent, options.Discard));

    return result.Id;
  }

  private async Task<T> SendCommand<T>(Command<T> command)
  {
    var result = await this._commandDispatcher.Execute(command);

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
