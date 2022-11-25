using System.Net.Http;
using SecTester.Bus.Commands;
using SecTester.Core;
using SecTester.Scan.CI;
using SecTester.Scan.Content;
using SecTester.Scan.Models;

namespace SecTester.Scan.Commands;

internal record CreateScan : HttpRequest<Identifiable<string>>
{
  public CreateScan(ScanConfig config, HttpContentFactory httpContentFactory, CiDiscovery ciDiscovery,
    Configuration configuration)
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
        client = new { configuration.Name, configuration.Version },
        Provider = ciDiscovery.Server?.ServerName
      }
    };

    Body = httpContentFactory.CreateJsonContent(payload);
  }
}
