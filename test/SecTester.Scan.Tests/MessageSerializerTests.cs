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

  [Theory]
  [MemberData(nameof(AttackParamLocationEnumerable))]
  [MemberData(nameof(FrameEnumerable))]
  [MemberData(nameof(ProtocolEnumerable))]
  [MemberData(nameof(DiscoveryEnumerable))]
  [MemberData(nameof(ModuleEnumerable))]
  [MemberData(nameof(ScanStatusEnumerable))]
  [MemberData(nameof(SeverityEnumerable))]
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
  public void Deserialize_GivenString_ReturnEnumValue(object expected, string input)
  {
    // act
    var result = MessageSerializer.Deserialize(input, expected.GetType());

    // assert
    result.Should().Be(expected);
  }
}
