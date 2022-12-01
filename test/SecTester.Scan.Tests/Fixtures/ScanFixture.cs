using Request = SecTester.Scan.Models.Request;

namespace SecTester.Scan.Tests.Fixtures;

internal class ScanFixture
{
  public const string BaseUrl = "https://example.com";
  public const string ScanName = "Scan Name";
  public const string ProjectId = "e9a2eX46EkidKhn3uqdYvE";
  public const string RepeaterId = "g5MvgM74sweGcK1U6hvs76";
  public const string FileId = "6aJa25Yd8DdXEcZg3QFoi8";
  public const string IssueId = "pDzxcEXQC8df1fcz1QwPf9";
  public const string ScanId = "roMq1UVuhPKkndLERNKnA8";
  public const string HarId = "gwycPnxzQihoeGP141pvDe";

  public static IEnumerable<object[]> DoneStatuses =>
    new List<object[]>
    {
      new object[] { ScanStatus.Disrupted },
      new object[] { ScanStatus.Done },
      new object[] { ScanStatus.Failed },
      new object[] { ScanStatus.Stopped }
    };

  public static IEnumerable<object[]> ActiveStatuses =>
    new List<object[]>
    {
      new object[] { ScanStatus.Pending },
      new object[] { ScanStatus.Running },
      new object[] { ScanStatus.Queued }
    };

  public static readonly ScanConfig ScanConfig = new(ScanName)
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

  public static readonly IEnumerable<Issue> Issues = new List<Issue>
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

  public static readonly Configuration Configuration = new("app.neuralegion.com");
}
