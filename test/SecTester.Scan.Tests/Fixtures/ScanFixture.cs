using NSubstitute.ClearExtensions;
using SecTester.Core.Bus;
using Request = SecTester.Scan.Models.Request;

namespace SecTester.Scan.Tests.Fixtures;

public class ScanFixture : IDisposable
{
  private const string BaseUrl = "https://example.com";
  private const string ScanName = "Scan Name";
  private const string ProjectId = "e9a2eX46EkidKhn3uqdYvE";
  private const string RepeaterId = "g5MvgM74sweGcK1U6hvs76";
  private const string FileId = "6aJa25Yd8DdXEcZg3QFoi8";
  private const string IssueId = "pDzxcEXQC8df1fcz1QwPf9";

  protected const string ScanId = "roMq1UVuhPKkndLERNKnA8";
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

  protected static readonly IEnumerable<Issue> Issues = new List<Issue>
  {
    new(IssueId,
      "Cross-site request forgery is a type of malicious website exploit.",
      "Database connection crashed",
      "The best way to protect against those kind of issues is making sure the Database resources are sufficient",
      new Request("https://brokencrystals.com/") { Method = HttpMethod.Get },
      new Request("https://brokencrystals.com/") { Method = HttpMethod.Get },
      $"{BaseUrl}/api/v1/scans/{ScanId}/issues/{IssueId}",
      1,
      Severity.Medium,
      Protocol.Http,
      DateTime.UtcNow) { Cvss = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:N/I:N/A:L" }
  };

  protected readonly Configuration Configuration = new("app.neuralegion.com");
  protected readonly CommandDispatcher CommandDispatcher = Substitute.For<CommandDispatcher>();
  protected readonly CiDiscovery CiDiscovery = Substitute.For<CiDiscovery>();

  public virtual void Dispose()
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
