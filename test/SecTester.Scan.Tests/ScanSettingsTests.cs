namespace SecTester.Scan.Tests;

public class ScanSettingsTests : IDisposable
{
  private const string ScanName = "Scan Name";
  private const string RepeaterId = "g5MvgM74sweGcK1U6hvs76";
  private const string TargetUrl = "https://example.com/api/v1/info";

  private readonly TargetOptions _targetOptions = Substitute.For<TargetOptions>();
  private readonly ScanSettingsOptions _scanSettingsOptions = Substitute.For<ScanSettingsOptions>();

  public static IEnumerable<object[]> SetterInvalidInput()
  {
    yield return new object[]
    {
      "Unknown attack param location supplied.", (ScanSettings x) =>
        x with { AttackParamLocations = new List<AttackParamLocation> { (AttackParamLocation)1024 } }
    };
    yield return new object[]
    {
      "Please provide at least one attack parameter location.", (ScanSettings x) =>
        x with { AttackParamLocations = new List<AttackParamLocation>() }
    };
    yield return new object[]
    {
      "Unknown test type supplied.", (ScanSettings x) =>
        x with { Tests = new List<TestType> { (TestType)1024 } }
    };
    yield return new object[]
    {
      "Please provide at least one test.", (ScanSettings x) =>
        x with { Tests = new List<TestType>() }
    };
    yield return new object[]
    {
      "Invalid target connection timeout.", (ScanSettings x) =>
        x with { TargetTimeout = null }
    };
    yield return new object[]
    {
      "Invalid target connection timeout.", (ScanSettings x) =>
        x with { TargetTimeout = TimeSpan.FromSeconds(0) }
    };
    yield return new object[]
    {
      "Invalid target connection timeout.", (ScanSettings x) =>
        x with { TargetTimeout = TimeSpan.FromSeconds(121) }
    };
    yield return new object[]
    {
      "Invalid slow entry point timeout.", (ScanSettings x) =>
        x with { SlowEpTimeout = null }
    };
    yield return new object[]
    {
      "Invalid slow entry point timeout.", (ScanSettings x) =>
        x with { SlowEpTimeout = TimeSpan.FromSeconds(99) }
    };
    yield return new object[]
    {
      "Invalid pool size.", (ScanSettings x) =>
        x with { PoolSize = null }
    };
    yield return new object[]
    {
      "Invalid pool size.", (ScanSettings x) =>
        x with { PoolSize = 0 }
    };
    yield return new object[]
    {
      "Invalid pool size.", (ScanSettings x) =>
        x with { PoolSize = 51 }
    };
    yield return new object[]
    {
      "Name must be less than 200 characters.", (ScanSettings x) =>
        x with { Name = null }
    };
    yield return new object[]
    {
      "Name must be less than 200 characters.", (ScanSettings x) =>
        x with { Name = " " }
    };
    yield return new object[]
    {
      "Name must be less than 200 characters.", (ScanSettings x) =>
        x with { Name = new string('a', 201) }
    };
  }

  public ScanSettingsTests()
  {
    _targetOptions.Url.Returns(TargetUrl);
    _targetOptions.Body.Returns(new StringContent("{}", Encoding.UTF8, "application/json"));
    _targetOptions.Headers.Returns(new List<KeyValuePair<string, IEnumerable<string>>>());
    _targetOptions.Method.Returns(HttpMethod.Get);
    _targetOptions.Query.Returns(new List<KeyValuePair<string, string>>());

    _scanSettingsOptions.Name.Returns(ScanName);
    _scanSettingsOptions.Smart.Returns(false);
    _scanSettingsOptions.Target.Returns(_targetOptions);
    _scanSettingsOptions.RepeaterId.Returns(RepeaterId);
    _scanSettingsOptions.PoolSize.Returns(1);
    _scanSettingsOptions.TargetTimeout.Returns(TimeSpan.FromSeconds(5));
    _scanSettingsOptions.SlowEpTimeout.Returns(TimeSpan.FromSeconds(200));
    _scanSettingsOptions.SkipStaticParams.Returns(false);
    _scanSettingsOptions.Tests.Returns(new List<TestType> { TestType.Csrf });
    _scanSettingsOptions.AttackParamLocations.Returns(new List<AttackParamLocation> { AttackParamLocation.Query });
  }

  public void Dispose()
  {
    _targetOptions.ClearSubstitute();
    _scanSettingsOptions.ClearSubstitute();
  }

  [Fact]
  public void PublicConstructor_CreatesInstanceWithDefaultOptions()
  {
    // act
    var result = new ScanSettings(_targetOptions, new List<TestType> { TestType.Csrf });

    // assert
    result.Should().BeEquivalentTo(new
    {
      RepeaterId = null as string,
      Name = "GET example.com",
      PoolSize = 10,
      TargetTimeout = TimeSpan.FromSeconds(5),
      SlowEpTimeout = TimeSpan.FromSeconds(1000),
      Smart = true,
      SkipStaticParams = true,
      Tests = new List<TestType> { TestType.Csrf },
      AttackParamLocations = new List<AttackParamLocation>
      {
        AttackParamLocation.Body, AttackParamLocation.Query, AttackParamLocation.Fragment
      },
      Target = new { Url = TargetUrl }
    });
  }

  [Fact]
  public void InternalConstructor_CreatesInstance()
  {
    // act
    var result = new ScanSettings(_scanSettingsOptions);

    // assert
    result.Should().BeEquivalentTo(new
    {
      RepeaterId,
      Name = ScanName,
      PoolSize = 1,
      TargetTimeout = TimeSpan.FromSeconds(5),
      SlowEpTimeout = TimeSpan.FromSeconds(200),
      Smart = false,
      SkipStaticParams = false,
      Tests = new List<TestType> { TestType.Csrf },
      AttackParamLocations = new List<AttackParamLocation> { AttackParamLocation.Query },
      Target = new { Url = TargetUrl }
    });
  }

  [Fact]
  public void InternalConstructor_NameIsNull_CreatesInstanceWithDefaultName()
  {
    // arrange
    _scanSettingsOptions.Name.ReturnsForAnyArgs(_ => null!);

    // act
    var result = new ScanSettings(_scanSettingsOptions);

    // assert
    result.Should().BeEquivalentTo(new { Name = "GET example.com" });
  }

  [Fact]
  public void InternalConstructor_NameIsNull_CreatesInstanceTruncatedHost()
  {
    // arrange
    _scanSettingsOptions.Name.ReturnsForAnyArgs(_ => null);
    _scanSettingsOptions.Target.Url.ReturnsForAnyArgs($"https://{new string('a', 200)}.example.com/api/v1/info");

    // act
    var result = new ScanSettings(_scanSettingsOptions);

    // assert
    result.Should().BeEquivalentTo(new { Name = $"GET {new string('a', 195)}…" });
  }

  [Fact]
  public void InternalConstructor_CreatesInstanceWithUniqueAttackParamLocations()
  {
    // arrange
    _scanSettingsOptions.AttackParamLocations.ReturnsForAnyArgs(
      new List<AttackParamLocation> { AttackParamLocation.Header, AttackParamLocation.Header });

    // act
    var result = new ScanSettings(_scanSettingsOptions);

    // assert
    result.Should().BeEquivalentTo(new
    {
      AttackParamLocations = new List<AttackParamLocation> { AttackParamLocation.Header }
    });
  }

  [Fact]
  public void InternalConstructor_CreatesInstanceWithUniqueTests()
  {
    // arrange
    _scanSettingsOptions.Tests.ReturnsForAnyArgs(
      new List<TestType> { TestType.Csrf, TestType.Csrf, TestType.Hrs });

    // act
    var result = new ScanSettings(_scanSettingsOptions);

    // assert
    result.Should().BeEquivalentTo(new { Tests = new List<TestType> { TestType.Csrf, TestType.Hrs } });
  }


  [Theory]
  [MemberData(nameof(SetterInvalidInput))]
  public void Setter_GivenInvalidInput_ThrowError(string expectedMessage,
    Func<ScanSettings, ScanSettings> action)
  {
    // arrange
    var sut = new ScanSettings(_scanSettingsOptions);

    // act
    var act = () => action(sut);

    // assert
    act.Should().Throw<ArgumentException>().WithMessage(expectedMessage);
  }
}
