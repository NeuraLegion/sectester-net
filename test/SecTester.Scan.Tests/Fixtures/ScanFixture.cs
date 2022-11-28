using NSubstitute.ClearExtensions;
using SecTester.Core;
using SecTester.Core.Bus;
using SecTester.Scan.CI;
using SecTester.Scan.Models;

namespace SecTester.Scan.Tests.Fixtures;

public class ScanFixture : IDisposable
{
  private const string ScanName = "Scan Name";
  private const string ProjectId = "e9a2eX46EkidKhn3uqdYvE";
  private const string RepeaterId = "g5MvgM74sweGcK1U6hvs76";
  private const string FileId = "6aJa25Yd8DdXEcZg3QFoi8";

  protected const string ScanId = "roMq1UVuhPKkndLERNKnA8";
  protected const string IssueId = "pDzxcEXQC8df1fcz1QwPf9";
  protected const string HarId = "gwycPnxzQihoeGP141pvDe";

  protected readonly ScanConfig ScanConfig = new(ScanName)
  {
    Module = Module.Dast,
    Repeaters = new[] { RepeaterId },
    Smart = true,
    Tests = new[] { TestType.Csrf, TestType.Jwt },
    DiscoveryTypes = new[] { Discovery.Crawler },
    FileId = FileId,
    HostsFilter = new[] { "example.com" },
    PoolSize = 2,
    ProjectId = ProjectId,
    TargetTimeout = 10,
    AttackParamLocations = new[] { AttackParamLocation.Body, AttackParamLocation.Header },
    SkipStaticParams = true,
    SlowEpTimeout = 20
  };

  protected readonly Configuration Configuration = new("app.neuralegion.com");
  protected readonly CommandDispatcher CommandDispatcher = Substitute.For<CommandDispatcher>();
  protected readonly CiDiscovery CiDiscovery = Substitute.For<CiDiscovery>();

  public void Dispose()
  {
    CiDiscovery.ClearSubstitute();
    CommandDispatcher.ClearSubstitute();

    GC.SuppressFinalize(this);
  }

  protected static string? ReadHttpContentAsString(HttpContent? content)
  {
    return content is null
      ? default
      : Task.Run(content.ReadAsStringAsync).GetAwaiter().GetResult();
  }
}
