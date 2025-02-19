namespace SecTester.Scan.Tests;

public class ScanSettingsTests
{
  private const string RepeaterId = "g5MvgM74sweGcK1U6hvs76";
  private const string Url = "https://example.com";
  private const string DefaultName = "GET example.com";

  private readonly Target _target = new(Url);
  private readonly IEnumerable<TestType> _tests = new List<TestType> { TestType.CrossSiteScripting };

  public static readonly IEnumerable<object[]> InvalidNames = new List<object[]>
  {
    new object[] { null!, "*Name*" },
    new object[] { "", "*Name*"  },
    new object[] { new string('a', ScanSettings.MaxNameLength + 1), $"Name must be less than {ScanSettings.MaxNameLength} characters." }
  };

  public static readonly IEnumerable<object[]> ValidSlowEpTimeout = new List<object[]>
  {
    new object[] { null! },
    new object[] { TimeSpan.FromSeconds(100) },
  };

  public static readonly IEnumerable<object[]> InvalidTargetTimeout = new List<object[]>
  {
    new object[] { TimeSpan.Zero },
    new object[] { TimeSpan.FromMinutes(121) },
  };

  public static readonly IEnumerable<object[]> ValidTargetTimeout = new List<object[]>
  {
    new object[] { null! },
    new object[] { TimeSpan.FromSeconds(100) },
  };

  public static readonly IEnumerable<object[]> InvalidTests = new List<object[]>
  {
    new object[] { null!, "*Tests*" },
    new object[] { new List<TestType> { (TestType) 1000 }, "Unknown test type supplied." },
    new object[] { Array.Empty<TestType>(), "Please provide at least one test."  }
  };

  public static readonly IEnumerable<object[]> InvalidAttackLocationParams = new List<object[]>
  {
    new object[] { new List<AttackParamLocation> { (AttackParamLocation) 1000 }, "Unknown attack param location supplied." },
    new object[] { Array.Empty<AttackParamLocation>(), "Please provide at least one attack parameter location."  }
  };

  [Theory]
  [MemberData(nameof(InvalidNames))]
  public void ScanSettings_ThrowsExceptionWhenNameIsInvalid(string input, string expectedErrorMessage)
  {
    // act
    var act = () => new ScanSettings(input, _target, _tests);

    // assert
    act.Should().Throw<Exception>()
      .WithMessage(expectedErrorMessage);
  }

  [Theory]
  [MemberData(nameof(InvalidTests))]
  public void ScanSettings_ThrowsExceptionWhenTestsIsInvalid(IEnumerable<TestType> input, string expectedErrorMessage)
  {
    // act
    var action = () => new ScanSettings(DefaultName, _target, input);

    // assert
    action.Should().Throw<Exception>()
      .WithMessage(expectedErrorMessage);
  }

  [Fact]
  public void ScanSettings_SetsUniqueTests()
  {
    // arrange
    var input = new List<TestType>
    {
      TestType.CrossSiteRequestForgery, TestType.CrossSiteRequestForgery
    };
    var expected = input.Distinct();

    // act
    var result = new ScanSettings(DefaultName, _target, input);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Tests = expected
    });
  }

  [Theory]
  [MemberData(nameof(InvalidAttackLocationParams))]
  public void ScanSettings_ThrowsExceptionWhenAttackParamLocationsIsInvalid(IEnumerable<AttackParamLocation> input, string expectedErrorMessage)
  {
    // act
    var action = () => new ScanSettings(DefaultName, _target, _tests)
    {
      AttackParamLocations = input
    };

    // assert
    action.Should().Throw<Exception>()
      .WithMessage(expectedErrorMessage);
  }

  [Fact]
  public void ScanSettings_AcceptsNullAsAttackParamLocations()
  {
    // act
    var result = new ScanSettings(DefaultName, _target, _tests)
    {
      AttackParamLocations = null
    };

    // assert
    result.AttackParamLocations.Should().BeNull();
  }

  [Fact]
  public void ScanSettings_SetsUniqueAttackParamLocations()
  {
    // arrange
    var input = new List<AttackParamLocation>
    {
      AttackParamLocation.ArtificalFragment, AttackParamLocation.ArtificalFragment
    };
    var expected = input.Distinct();

    // act
    var result = new ScanSettings(DefaultName, _target, _tests)
    {
      AttackParamLocations = input
    };

    // assert
    result.Should().BeEquivalentTo(new
    {
      AttackParamLocations = expected
    });
  }

  [Theory]
  [InlineData(0)]
  [InlineData(51)]
  public void ScanSettings_ThrowsExceptionWhenPoolSizeIsInvalid(int? input)
  {
    // act
    var act = () => new ScanSettings(DefaultName, _target, _tests)
    {
      PoolSize = input
    };

    // assert
    act.Should().Throw<ArgumentException>().WithMessage("Invalid pool size.");
  }

  [Theory]
  [InlineData(25)]
  [InlineData(null)]
  public void ScanSettings_SetsPoolSize(int? input)
  {
    // act
    var result = new ScanSettings(DefaultName, _target, _tests)
    {
      PoolSize = input
    };

    // assert
    result.Should().BeEquivalentTo(new
    {
      PoolSize = input
    });
  }

  [Fact]
  public void ScanSettings_ThrowsExceptionWhenSlowEpTimeoutIsInvalid()
  {
    // act
    var act = () => new ScanSettings(DefaultName, _target, _tests)
    {
      SlowEpTimeout = TimeSpan.Zero

    };

    // assert
    act.Should().Throw<ArgumentException>().WithMessage("Invalid slow entry point timeout.");
  }

  [Theory]
  [MemberData(nameof(ValidSlowEpTimeout))]
  public void ScanSettings_SetsSlowEpTimeout(TimeSpan? input)
  {
    // act
    var result = new ScanSettings(DefaultName, _target, _tests)
    {
      SlowEpTimeout = input
    };

    // assert
    result.Should().BeEquivalentTo(new
    {
      SlowEpTimeout = input
    });
  }

  [Theory]
  [MemberData(nameof(InvalidTargetTimeout))]
  public void ScanSettings_ThrowsExceptionWhenTargetTimeoutIsInvalid(TimeSpan? input)
  {
    // act
    var act = () => new ScanSettings(DefaultName, _target, _tests)
    {
      TargetTimeout = input
    };

    // assert
    act.Should().Throw<ArgumentException>().WithMessage("Invalid target connection timeout.");
  }

  [Theory]
  [MemberData(nameof(ValidTargetTimeout))]
  public void ScanSettings_SetsTargetTimeout(TimeSpan? input)
  {
    // act
    var result = new ScanSettings(DefaultName, _target, _tests)
    {
      TargetTimeout = input
    };

    // assert
    result.Should().BeEquivalentTo(new
    {
      TargetTimeout = input
    });
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void ScanSettings_SetsSkipStaticParams(bool input)
  {
    // act
    var result = new ScanSettings(DefaultName, _target, _tests)
    {
      SkipStaticParams = input
    };

    // assert
    result.Should().BeEquivalentTo(new
    {
      SkipStaticParams = input
    });
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void ScanSettings_SetsSmart(bool input)
  {
    // act
    var result = new ScanSettings(DefaultName, _target, _tests)
    {
      Smart = input
    };

    // assert
    result.Should().BeEquivalentTo(new
    {
      Smart = input
    });
  }

  [Theory]
  [InlineData(RepeaterId)]
  [InlineData(null)]
  public void ScanSettings_SetsRepeaterId(string? input)
  {
    // act
    var result = new ScanSettings(DefaultName, _target, _tests)
    {
      RepeaterId = input
    };

    // assert
    result.Should().BeEquivalentTo(new
    {
      RepeaterId = input
    });
  }

  [Fact]
  public void ScanSettings_ThrowsExceptionWhenTargetIsNull()
  {
    // act
    var act = () => new ScanSettings(DefaultName, null!, _tests);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*Target*");
  }
}
