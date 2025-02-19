namespace SecTester.Scan.Tests;

public class MessageSerializerTests
{
  public static readonly IEnumerable<object[]> AttackParamLocationEnumerable = new List<object[]>
  {
    new object[] { AttackParamLocation.ArtificalFragment, @"""artifical-fragment""" },
    new object[] { AttackParamLocation.ArtificalQuery, @"""artifical-query""" },
    new object[] { AttackParamLocation.Body, @"""body""" },
    new object[] { AttackParamLocation.Fragment, @"""fragment""" },
    new object[] { AttackParamLocation.Header, @"""header""" },
    new object[] { AttackParamLocation.Path, @"""path""" },
    new object[] { AttackParamLocation.Query, @"""query""" }
  };

  public static readonly IEnumerable<object[]> FrameEnumerable = new List<object[]>
  {
    new object[] { Frame.Incoming, @"""incoming""" },
    new object[] { Frame.Outgoing, @"""outgoing""" }
  };

  public static readonly IEnumerable<object[]> ProtocolEnumerable = new List<object[]>
  {
    new object[] { Protocol.Http, @"""http""" },
    new object[] { Protocol.Ws, @"""ws""" }
  };

  public static readonly IEnumerable<object[]> DiscoveryEnumerable = new List<object[]>
  {
    new object[] { Discovery.Crawler, @"""crawler""" },
    new object[] { Discovery.Archive, @"""archive""" },
    new object[] { Discovery.Oas, @"""oas""" }
  };

  public static readonly IEnumerable<object[]> ModuleEnumerable = new List<object[]>
  {
    new object[] { Module.Dast, @"""dast""" },
    new object[] { Module.Fuzzer, @"""fuzzer""" }
  };

  public static readonly IEnumerable<object[]> ScanStatusEnumerable = new List<object[]>
  {
    new object[] { ScanStatus.Failed, @"""failed""" },
    new object[] { ScanStatus.Disrupted, @"""disrupted""" },
    new object[] { ScanStatus.Running, @"""running""" },
    new object[] { ScanStatus.Stopped, @"""stopped""" },
    new object[] { ScanStatus.Queued, @"""queued""" },
    new object[] { ScanStatus.Scheduled, @"""scheduled""" },
    new object[] { ScanStatus.Pending, @"""pending""" },
    new object[] { ScanStatus.Done, @"""done""" },
    new object[] { ScanStatus.Paused, @"""paused""" }
  };

  public static readonly IEnumerable<object[]> SeverityEnumerable = new List<object[]>
  {
    new object[] { Severity.Critical, @"""Critical""" },
    new object[] { Severity.Medium, @"""Medium""" },
    new object[] { Severity.High, @"""High""" },
    new object[] { Severity.Low, @"""Low""" }
  };

  public static readonly IEnumerable<object[]> TestTypeEnumerable = new List<object[]>
  {
    new object[] { TestType.AmazonS3Takeover, @"""amazon_s3_takeover""" },
    new object[] { TestType.BrokenObjectPropertyLevelAuthorization, @"""bopla""" },
    new object[] { TestType.BrokenAccessControl, @"""broken_access_control""" },
    new object[] { TestType.BrokenSamlAuthentication, @"""broken_saml_auth""" },
    new object[] { TestType.BrokenJwtAuthentication, @"""jwt""" },
    new object[] { TestType.BruteForceLogin, @"""brute_force_login""" },
    new object[] { TestType.BusinessConstraintBypass, @"""business_constraint_bypass""" },
    new object[] { TestType.CookieSecurity, @"""cookie_security""" },
    new object[] { TestType.CrossSiteRequestForgery, @"""csrf""" },
    new object[] { TestType.CssInjection, @"""css_injection""" },
    new object[] { TestType.DateManipulation, @"""date_manipulation""" },
    new object[] { TestType.EmailInjection, @"""email_injection""" },
    new object[] { TestType.ExcessiveDataExposure, @"""excessive_data_exposure""" },
    new object[] { TestType.FileUpload, @"""file_upload""" },
    new object[] { TestType.FullPathDisclosure, @"""full_path_disclosure""" },
    new object[] { TestType.GraphqlIntrospection, @"""graphql_introspection""" },
    new object[] { TestType.HtmlInjection, @"""html_injection""" },
    new object[] { TestType.HttpMethodFuzzing, @"""http_method_fuzzing""" },
    new object[] { TestType.IdEnumeration, @"""id_enumeration""" },
    new object[] { TestType.IframeInjection, @"""iframe_injection""" },
    new object[] { TestType.ImproperAssetManagement, @"""improper_asset_management""" },
    new object[] { TestType.InsecureOutputHandling, @"""insecure_output_handling""" },
    new object[] { TestType.LdapInjection, @"""ldapi""" },
    new object[] { TestType.LocalFileInclusion, @"""lfi""" },
    new object[] { TestType.MassAssignment, @"""mass_assignment""" },
    new object[] { TestType.MongodbInjection, @"""nosql""" },
    new object[] { TestType.OpenCloudStorage, @"""open_cloud_storage""" },
    new object[] { TestType.ExposedDatabaseDetails, @"""open_database""" },
    new object[] { TestType.OsCommandInjection, @"""osi""" },
    new object[] { TestType.PasswordResetPoisoning, @"""password_reset_poisoning""" },
    new object[] { TestType.PromptInjection, @"""prompt_injection""" },
    new object[] { TestType.JsPrototypePollution, @"""proto_pollution""" },
    new object[] { TestType.RemoteFileInclusion, @"""rfi""" },
    new object[] { TestType.SqlInjection, @"""sqli""" },
    new object[] { TestType.SecretTokensLeak, @"""secret_tokens""" },
    new object[] { TestType.ServerSideJsInjection, @"""server_side_js_injection""" },
    new object[] { TestType.ServerSideRequestForgery, @"""ssrf""" },
    new object[] { TestType.ServerSideTemplateInjection, @"""ssti""" },
    new object[] { TestType.StoredCrossSiteScripting, @"""stored_xss""" },
    new object[] { TestType.UnvalidatedRedirect, @"""unvalidated_redirect""" },
    new object[] { TestType.XpathInjection, @"""xpathi""" },
    new object[] { TestType.XmlExternalEntityInjection, @"""xxe""" },
    new object[] { TestType.CrossSiteScripting, @"""xss""" }
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
  public void Serialize_GivenEnumValue_ReturnString(object input, string expected)
  {
    // act
    var data = MessageSerializer.Serialize(input);

    // assert
    data.Should().Be(expected);
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
  public void Deserialize_GivenString_ReturnEnumValue(object expected, string input)
  {
    // act
    var result = MessageSerializer.Deserialize(input, expected.GetType());

    // assert
    result.Should().Be(expected);
  }
}
