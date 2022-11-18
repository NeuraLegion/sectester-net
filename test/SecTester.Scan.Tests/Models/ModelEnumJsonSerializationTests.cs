using SecTester.Bus.Dispatchers;
using Module = SecTester.Scan.Models.Module;

namespace SecTester.Scan.Tests.Models;

public class JsonStringEnumMemberConverterTests
{
  private readonly DefaultMessageSerializer _sut = new();

  public static readonly IEnumerable<object[]> AttackParamLocationEnumerable = new List<object[]>()
  {
    new object[] { AttackParamLocation.ArtificalFragment, @"""artifical-fragment""", typeof(AttackParamLocation) },
    new object[] { AttackParamLocation.ArtificalQuery, @"""artifical-query""", typeof(AttackParamLocation) },
    new object[] { AttackParamLocation.Body, @"""body""", typeof(AttackParamLocation) },
    new object[] { AttackParamLocation.Fragment, @"""fragment""", typeof(AttackParamLocation) },
    new object[] { AttackParamLocation.Header, @"""header""", typeof(AttackParamLocation) },
    new object[] { AttackParamLocation.Path, @"""path""", typeof(AttackParamLocation) },
    new object[] { AttackParamLocation.Query, @"""query""", typeof(AttackParamLocation) }
  };

  public static readonly IEnumerable<object[]> FrameEnumerable = new List<object[]>()
  {
    new object[] { Frame.Incoming, @"""incoming""", typeof(Frame) },
    new object[] { Frame.Outgoing, @"""outgoing""", typeof(Frame) },
  };

  public static readonly IEnumerable<object[]> ProtocolEnumerable = new List<object[]>()
  {
    new object[] { Protocol.Http, @"""http""", typeof(Protocol) },
    new object[] { Protocol.Ws, @"""ws""", typeof(Protocol) },
  };

  public static readonly IEnumerable<object[]> DiscoveryEnumerable = new List<object[]>()
  {
    new object[] { Discovery.Crawler, @"""crawler""", typeof(Discovery) },
    new object[] { Discovery.Archive, @"""archive""", typeof(Discovery) },
    new object[] { Discovery.Oas, @"""oas""", typeof(Discovery) }
  };

  public static readonly IEnumerable<object[]> ModuleEnumerable = new List<object[]>()
  {
    new object[] { Module.Dast, @"""dast""", typeof(Module) },
    new object[] { Module.Fuzzer, @"""fuzzer""", typeof(Module) },
  };

  public static readonly IEnumerable<object[]> ScanStatusEnumerable = new List<object[]>()
  {
    new object[] { ScanStatus.Failed, @"""failed""", typeof(ScanStatus) },
    new object[] { ScanStatus.Disrupted, @"""disrupted""", typeof(ScanStatus) },
    new object[] { ScanStatus.Running, @"""running""", typeof(ScanStatus) },
    new object[] { ScanStatus.Stopped, @"""stopped""", typeof(ScanStatus) },
    new object[] { ScanStatus.Queued, @"""queued""", typeof(ScanStatus) },
    new object[] { ScanStatus.Scheduled, @"""scheduled""", typeof(ScanStatus) },
    new object[] { ScanStatus.Pending, @"""pending""", typeof(ScanStatus) },
    new object[] { ScanStatus.Done, @"""done""", typeof(ScanStatus) },
    new object[] { ScanStatus.Paused, @"""paused""", typeof(ScanStatus) }
  };

  public static readonly IEnumerable<object[]> SeverityEnumerable = new List<object[]>()
  {
    new object[] { Severity.Medium, @"""Medium""", typeof(Severity) },
    new object[] { Severity.High, @"""High""", typeof(Severity) },
    new object[] { Severity.Low, @"""Low""", typeof(Severity) }
  };

  public static readonly IEnumerable<object[]> TestTypeEnumerable = new List<object[]>()
  {
    new object[] { TestType.AngularCsti, @"""angular_csti""", typeof(TestType) },
    new object[] { TestType.BackupLocations, @"""backup_locations""", typeof(TestType) },
    new object[] { TestType.BrokenSamlAuth, @"""broken_saml_auth""", typeof(TestType) },
    new object[] { TestType.BruteForceLogin, @"""brute_force_login""", typeof(TestType) },
    new object[] { TestType.BusinessConstraintBypass, @"""business_constraint_bypass""", typeof(TestType) },
    new object[] { TestType.CommonFiles, @"""common_files""", typeof(TestType) },
    new object[] { TestType.CookieSecurity, @"""cookie_security""", typeof(TestType) },
    new object[] { TestType.Csrf, @"""csrf""", typeof(TestType) },
    new object[] { TestType.DateManipulation, @"""date_manipulation""", typeof(TestType) },
    new object[] { TestType.DefaultLoginLocation, @"""default_login_location""", typeof(TestType) },
    new object[] { TestType.DirectoryListing, @"""directory_listing""", typeof(TestType) },
    new object[] { TestType.DomXss, @"""dom_xss""", typeof(TestType) },
    new object[] { TestType.EmailInjection, @"""email_injection""", typeof(TestType) },
    new object[] { TestType.ExposedCouchDbApis, @"""exposed_couch_db_apis""", typeof(TestType) },
    new object[] { TestType.FileUpload, @"""file_upload""", typeof(TestType) },
    new object[] { TestType.FullPathDisclosure, @"""full_path_disclosure""", typeof(TestType) },
    new object[] { TestType.HeaderSecurity, @"""header_security""", typeof(TestType) },
    new object[] { TestType.Hrs, @"""hrs""", typeof(TestType) },
    new object[] { TestType.HtmlInjection, @"""html_injection""", typeof(TestType) },
    new object[] { TestType.HttpMethodFuzzing, @"""http_method_fuzzing""", typeof(TestType) },
    new object[] { TestType.HttpResponseSplitting, @"""http_response_splitting""", typeof(TestType) },
    new object[] { TestType.IdEnumeration, @"""id_enumeration""", typeof(TestType) },
    new object[] { TestType.ImproperAssetManagement, @"""improper_asset_management""", typeof(TestType) },
    new object[] { TestType.InsecureTlsConfiguration, @"""insecure_tls_configuration""", typeof(TestType) },
    new object[] { TestType.Jwt, @"""jwt""", typeof(TestType) },
    new object[] { TestType.Ldapi, @"""ldapi""", typeof(TestType) },
    new object[] { TestType.Lfi, @"""lfi""", typeof(TestType) },
    new object[] { TestType.MassAssignment, @"""mass_assignment""", typeof(TestType) },
    new object[] { TestType.Nosql, @"""nosql""", typeof(TestType) },
    new object[] { TestType.OpenBuckets, @"""open_buckets""", typeof(TestType) },
    new object[] { TestType.OpenDatabase, @"""open_database""", typeof(TestType) },
    new object[] { TestType.Osi, @"""osi""", typeof(TestType) },
    new object[] { TestType.ProtoPollution, @"""proto_pollution""", typeof(TestType) },
    new object[] { TestType.RetireJs, @"""retire_js""", typeof(TestType) },
    new object[] { TestType.Rfi, @"""rfi""", typeof(TestType) },
    new object[] { TestType.SecretTokens, @"""secret_tokens""", typeof(TestType) },
    new object[] { TestType.ServerSideJsInjection, @"""server_side_js_injection""", typeof(TestType) },
    new object[] { TestType.Sqli, @"""sqli""", typeof(TestType) },
    new object[] { TestType.Ssrf, @"""ssrf""", typeof(TestType) },
    new object[] { TestType.Ssti, @"""ssti""", typeof(TestType) },
    new object[] { TestType.UnvalidatedRedirect, @"""unvalidated_redirect""", typeof(TestType) },
    new object[] { TestType.VersionControlSystems, @"""version_control_systems""", typeof(TestType) },
    new object[] { TestType.Wordpress, @"""wordpress""", typeof(TestType) },
    new object[] { TestType.Xpathi, @"""xpathi""", typeof(TestType) },
    new object[] { TestType.Xss, @"""xss""", typeof(TestType) },
    new object[] { TestType.Xxe, @"""xxe""", typeof(TestType) }
  };


  [Theory]
  [MemberData(nameof(AttackParamLocationEnumerable))]
  [MemberData(nameof(FrameEnumerable))]
  [MemberData(nameof(ProtocolEnumerable))]
  [MemberData(nameof(DiscoveryEnumerable))]
  [MemberData(nameof(ModuleEnumerable))]
  [MemberData(nameof(ScanStatusEnumerable))]
  [MemberData(nameof(SeverityEnumerable))]
  [MemberData(nameof(TestTypeEnumerable))]
  public void Serialize_GivenEnumValue_ReturnString(object value, string valueString, Type type)
  {
    // act
    var data = _sut.Serialize(value);

    // assert
    data.Should().Be(valueString);
  }

  [Theory]
  [MemberData(nameof(AttackParamLocationEnumerable))]
  [MemberData(nameof(FrameEnumerable))]
  [MemberData(nameof(ProtocolEnumerable))]
  [MemberData(nameof(DiscoveryEnumerable))]
  [MemberData(nameof(ModuleEnumerable))]
  [MemberData(nameof(ScanStatusEnumerable))]
  [MemberData(nameof(SeverityEnumerable))]
  [MemberData(nameof(TestTypeEnumerable))]
  public void Deserialize_GivenString_ReturnEnumValue(object value, string valueString, Type type)
  {
    // act
    var result = _sut.Deserialize(valueString, type);

    // assert
    result.Should().Be(value);
  }
}
