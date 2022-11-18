using SecTester.Bus.Tests.Fixtures;
using SecTester.Scan.Models;

namespace SecTester.Bus.Tests.Dispatchers;

public class DefaultMessageSerializerTests
{
  private readonly DefaultMessageSerializer _sut;

  public DefaultMessageSerializerTests()
  {
    _sut = new DefaultMessageSerializer();
  }

  public static IEnumerable<object[]> Objects => new List<object[]>
  {
    new object[] { new { foo = "bar" }, @"{""foo"":""bar""}" },
    new object[] { new { Foo = "bar" }, @"{""foo"":""bar""}" },
    new object[] { new FooBar("bar"), @"{""foo"":""bar""}" }
  };

  public static IEnumerable<object[]> Strings => new List<object[]>
  {
    new object[] { @"{""foo"":""bar""}" },
    new object[] { @"{""Foo"":""bar""}" },
    new object[] { @"{""FoO"":""bar""}" }
  };
  
  public static IEnumerable<object[]> EnumValues => new List<object[]>
  {
    new object[] { FooEnum.Bar, @"""bar""" },
    new object[] { FooEnum.FooBar, @"""foo_bar""" },
    new object[] { FooEnum.BazQux, @"""baz-qux""" },
  };
  
    public static readonly IEnumerable<object[]> AttackParamLocationEnumerable = new List<object[]>
  {
    new object[] { AttackParamLocation.ArtificalFragment, @"""artifical-fragment"""},
    new object[] { AttackParamLocation.ArtificalQuery, @"""artifical-query"""},
    new object[] { AttackParamLocation.Body, @"""body"""},
    new object[] { AttackParamLocation.Fragment, @"""fragment"""},
    new object[] { AttackParamLocation.Header, @"""header"""},
    new object[] { AttackParamLocation.Path, @"""path"""},
    new object[] { AttackParamLocation.Query, @"""query"""}
  };

  public static readonly IEnumerable<object[]> FrameEnumerable = new List<object[]>
  {
    new object[] { Frame.Incoming, @"""incoming"""},
    new object[] { Frame.Outgoing, @"""outgoing"""},
  };

  public static readonly IEnumerable<object[]> ProtocolEnumerable = new List<object[]>
  {
    new object[] { Protocol.Http, @"""http"""},
    new object[] { Protocol.Ws, @"""ws"""},
  };

  public static readonly IEnumerable<object[]> DiscoveryEnumerable = new List<object[]>
  {
    new object[] { Discovery.Crawler, @"""crawler"""},
    new object[] { Discovery.Archive, @"""archive"""},
    new object[] { Discovery.Oas, @"""oas"""}
  };

  public static readonly IEnumerable<object[]> ModuleEnumerable = new List<object[]>
  {
    new object[] { Module.Dast, @"""dast"""},
    new object[] { Module.Fuzzer, @"""fuzzer"""},
  };

  public static readonly IEnumerable<object[]> ScanStatusEnumerable = new List<object[]>
  {
    new object[] { ScanStatus.Failed, @"""failed"""},
    new object[] { ScanStatus.Disrupted, @"""disrupted"""},
    new object[] { ScanStatus.Running, @"""running"""},
    new object[] { ScanStatus.Stopped, @"""stopped"""},
    new object[] { ScanStatus.Queued, @"""queued"""},
    new object[] { ScanStatus.Scheduled, @"""scheduled"""},
    new object[] { ScanStatus.Pending, @"""pending"""},
    new object[] { ScanStatus.Done, @"""done"""},
    new object[] { ScanStatus.Paused, @"""paused"""}
  };

  public static readonly IEnumerable<object[]> SeverityEnumerable = new List<object[]>
  {
    new object[] { Severity.Medium, @"""Medium"""},
    new object[] { Severity.High, @"""High"""},
    new object[] { Severity.Low, @"""Low"""}
  };

  public static readonly IEnumerable<object[]> TestTypeEnumerable = new List<object[]>
  {
    new object[] { TestType.AngularCsti, @"""angular_csti"""},
    new object[] { TestType.BackupLocations, @"""backup_locations"""},
    new object[] { TestType.BrokenSamlAuth, @"""broken_saml_auth"""},
    new object[] { TestType.BruteForceLogin, @"""brute_force_login"""},
    new object[] { TestType.BusinessConstraintBypass, @"""business_constraint_bypass"""},
    new object[] { TestType.CommonFiles, @"""common_files"""},
    new object[] { TestType.CookieSecurity, @"""cookie_security"""},
    new object[] { TestType.Csrf, @"""csrf"""},
    new object[] { TestType.DateManipulation, @"""date_manipulation"""},
    new object[] { TestType.DefaultLoginLocation, @"""default_login_location"""},
    new object[] { TestType.DirectoryListing, @"""directory_listing"""},
    new object[] { TestType.DomXss, @"""dom_xss"""},
    new object[] { TestType.EmailInjection, @"""email_injection"""},
    new object[] { TestType.ExposedCouchDbApis, @"""exposed_couch_db_apis"""},
    new object[] { TestType.FileUpload, @"""file_upload"""},
    new object[] { TestType.FullPathDisclosure, @"""full_path_disclosure"""},
    new object[] { TestType.HeaderSecurity, @"""header_security"""},
    new object[] { TestType.Hrs, @"""hrs"""},
    new object[] { TestType.HtmlInjection, @"""html_injection"""},
    new object[] { TestType.HttpMethodFuzzing, @"""http_method_fuzzing"""},
    new object[] { TestType.HttpResponseSplitting, @"""http_response_splitting"""},
    new object[] { TestType.IdEnumeration, @"""id_enumeration"""},
    new object[] { TestType.ImproperAssetManagement, @"""improper_asset_management"""},
    new object[] { TestType.InsecureTlsConfiguration, @"""insecure_tls_configuration"""},
    new object[] { TestType.Jwt, @"""jwt"""},
    new object[] { TestType.Ldapi, @"""ldapi"""},
    new object[] { TestType.Lfi, @"""lfi"""},
    new object[] { TestType.MassAssignment, @"""mass_assignment"""},
    new object[] { TestType.Nosql, @"""nosql"""},
    new object[] { TestType.OpenBuckets, @"""open_buckets"""},
    new object[] { TestType.OpenDatabase, @"""open_database"""},
    new object[] { TestType.Osi, @"""osi"""},
    new object[] { TestType.ProtoPollution, @"""proto_pollution"""},
    new object[] { TestType.RetireJs, @"""retire_js"""},
    new object[] { TestType.Rfi, @"""rfi"""},
    new object[] { TestType.SecretTokens, @"""secret_tokens"""},
    new object[] { TestType.ServerSideJsInjection, @"""server_side_js_injection"""},
    new object[] { TestType.Sqli, @"""sqli"""},
    new object[] { TestType.Ssrf, @"""ssrf"""},
    new object[] { TestType.Ssti, @"""ssti"""},
    new object[] { TestType.UnvalidatedRedirect, @"""unvalidated_redirect"""},
    new object[] { TestType.VersionControlSystems, @"""version_control_systems"""},
    new object[] { TestType.Wordpress, @"""wordpress"""},
    new object[] { TestType.Xpathi, @"""xpathi"""},
    new object[] { TestType.Xss, @"""xss"""},
    new object[] { TestType.Xxe, @"""xxe"""}
  };
  
  public static readonly IEnumerable<object[]> HttpMethodEnumerable = new List<object[]>
  {
    new object[] { HttpMethod.Delete, @"""DELETE"""},
    new object[] { HttpMethod.Get, @"""GET"""},
    new object[] { HttpMethod.Head, @"""HEAD"""},
    new object[] { HttpMethod.Options, @"""OPTIONS"""},
    new object[] { HttpMethod.Patch, @"""PATCH"""},
    new object[] { HttpMethod.Post, @"""POST"""},
    new object[] { HttpMethod.Put, @"""PUT"""},
    new object[] { HttpMethod.Trace, @"""TRACE"""},
    new object[] { new HttpMethod("COPY"), @"""COPY"""},
    new object[] { new HttpMethod("LINK"), @"""LINK"""},
    new object[] { new HttpMethod("UNLINK"), @"""UNLINK"""},
    new object[] { new HttpMethod("PURGE"), @"""PURGE"""},
    new object[] { new HttpMethod("LOCK"), @"""LOCK"""},
    new object[] { new HttpMethod("UNLOCK"), @"""UNLOCK"""},
    new object[] { new HttpMethod("PROPFIND"), @"""PROPFIND"""},
    new object[] { new HttpMethod("VIEW"), @"""VIEW"""}
  };

  [Theory]
  [MemberData(nameof(Objects))]
  public void Serialize_GivenInput_ReturnsString(object input, string expected)
  {
    // act
    var result = _sut.Serialize(input);

    // assert
    result.Should().Be(expected);
  }

  [Theory]
  [MemberData(nameof(Strings))]
  public void Deserialize_GivenType_ReturnsObject(string input)
  {
    // act
    var result = _sut.Deserialize(input, typeof(FooBar));

    // assert
    result.Should().BeOfType<FooBar>();
    result.Should().BeEquivalentTo(new
    {
      Foo = "bar"
    });
  }

  [Theory]
  [MemberData(nameof(Strings))]
  public void Deserialize_GenericReturnType_ReturnsObject(string input)
  {
    // act
    var result = _sut.Deserialize<FooBar>(input);

    // assert
    result.Should().BeOfType<FooBar>();
    result.Should().BeEquivalentTo(new
    {
      Foo = "bar"
    });
  }
  
  [Theory]
  [MemberData(nameof(EnumValues))]
  [MemberData(nameof(AttackParamLocationEnumerable))]
  [MemberData(nameof(FrameEnumerable))]
  [MemberData(nameof(ProtocolEnumerable))]
  [MemberData(nameof(DiscoveryEnumerable))]
  [MemberData(nameof(ModuleEnumerable))]
  [MemberData(nameof(ScanStatusEnumerable))]
  [MemberData(nameof(SeverityEnumerable))]
  [MemberData(nameof(TestTypeEnumerable))]
  [MemberData(nameof(HttpMethodEnumerable))]
  public void Serialize_GivenEnumValue_ReturnString(object input, string expected)
  {
    // act
    var data = _sut.Serialize(input);

    // assert
    data.Should().Be(expected);
  }

  [Theory]
  [MemberData(nameof(EnumValues))]
  [MemberData(nameof(AttackParamLocationEnumerable))]
  [MemberData(nameof(FrameEnumerable))]
  [MemberData(nameof(ProtocolEnumerable))]
  [MemberData(nameof(DiscoveryEnumerable))]
  [MemberData(nameof(ModuleEnumerable))]
  [MemberData(nameof(ScanStatusEnumerable))]
  [MemberData(nameof(SeverityEnumerable))]
  [MemberData(nameof(TestTypeEnumerable))]
  [MemberData(nameof(HttpMethodEnumerable))]
  public void Deserialize_GivenString_ReturnEnumValue(object expected, string input)
  {
    // act
    var result = _sut.Deserialize(input, expected.GetType());

    // assert
    result.Should().Be(expected);
  }
  
  [Fact]
  public void Deserialize_GivenMissingFieldInput_ReturnObject()
  {
    // act
    var result = _sut.Deserialize<Tuple<FooEnum?>>("{}");

    // assert
    result.Should().BeOfType<Tuple<FooEnum?>>();
    result!.Item1.Should().BeNull();
  }
  
  [Fact]
  public void Deserialize_GivenNullFieldInput_ReturnObject()
  {
    // act
    var result = _sut.Deserialize<Tuple<FooEnum?>>(@"{""item1"":null}");

    // assert
    result.Should().BeOfType<Tuple<FooEnum?>>();
    result!.Item1.Should().BeNull();
  }

  [Fact]
  public void Deserialize_GivenMissingMember_ThrowError()
  {
    // act
    var act = () =>_sut.Deserialize(@"""FOO""", typeof(HttpMethod));

    // assert
    act.Should().Throw<JsonException>().WithMessage("*FOO*");
  }
  
  [Fact]
  public void Serialize_GivenMissingMember_ThrowError()
  {
    // act
    var act = () => _sut.Serialize(new HttpMethod("FOO"));

    // assert
    act.Should().Throw<JsonException>().WithMessage("*FOO*");
  }
  
}
