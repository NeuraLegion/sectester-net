using System.Net.Http;
using System.Text;
using SecTester.Core.Commands;
using SecTester.Core.Dispatchers;
using SecTester.Scan.Models;

namespace SecTester.Scan.Commands;

internal record CreateScan : HttpRequest<Identifiable<string>>
{
  public CreateScan(ScanConfig config, string configurationName, string configurationVersion, string? ciProvider)
    : base("/api/v1/scans", HttpMethod.Post)
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
        Source = "utlib",
        client = new
        {
          Name = configurationName,
          Version = configurationVersion
        },
        Provider = ciProvider
      }
    };

    Body = new StringContent(MessageSerializer.Serialize(payload), Encoding.UTF8, "application/json");
  }
}
