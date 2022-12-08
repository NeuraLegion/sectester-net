using SecTester.Scan.Tests.Fixtures;

namespace SecTester.Scan.Tests;

public class ScanSettingsTests
{
  private const string ScanName = "Scan Name";
  private const string RepeaterId = "g5MvgM74sweGcK1U6hvs76";
  private const string TargetUrl = "https://example.com/api/v1/info";

  public static readonly IEnumerable<object[]> DefaultNameInput = new List<object[]>
  {
    new object[]
    {
      "GET example.com",
      (TestScanSettingsOptions options) =>
        new ScanSettings(options with
        {
          Name = null
        })
    },
    new object[]
    {
      "GET example.com",
      (TestScanSettingsOptions options) =>
        new ScanSettings(options.Target, new List<TestType>
        {
          TestType.Hrs
        })
    },
    new object[]
    {
      $"GET {new string('a', ScanSettings.MaxNameLength - 5)}…",
      (TestScanSettingsOptions options) =>
        new ScanSettings(options with
        {
          Name = null,
          Target = (TestTargetOptions)options.Target with
          {
            Url = $"https://{new string('a', ScanSettings.MaxNameLength)}.example.com/api/v1/info"
          }
        })
    },
    new object[]
    {
      $"GET {new string('a', ScanSettings.MaxNameLength - 5)}…",
      (TestScanSettingsOptions options) =>
        new ScanSettings(
          (TestTargetOptions)options.Target with
          {
            Url = $"https://{new string('a', ScanSettings.MaxNameLength)}.example.com/api/v1/info"
          }, new List<TestType>
          {
            TestType.Hrs
          })
    }
  };

  public static readonly IEnumerable<object[]> UniqueAttackParamLocationsInput = new List<object[]>
  {
    new object[]
    {
      (TestScanSettingsOptions options, IEnumerable<AttackParamLocation> @params) =>
        new ScanSettings(options with
        {
          AttackParamLocations = @params
        })
    },
    new object[]
    {
      (TestScanSettingsOptions options, IEnumerable<AttackParamLocation> @params) =>
        new ScanSettings(options)
        {
          AttackParamLocations = @params
        }
    }
  };

  public static readonly IEnumerable<object[]> UniqueTestTypesInput = new List<object[]>
  {
    new object[]
    {
      (TestScanSettingsOptions options, IEnumerable<TestType> @params) =>
        new ScanSettings(options with
        {
          Tests = @params
        })
    },
    new object[]
    {
      (TestScanSettingsOptions options, IEnumerable<TestType> @params) =>
        new ScanSettings(options)
        {
          Tests = @params
        }
    },
    new object[]
    {
      (TestScanSettingsOptions options, IEnumerable<TestType> @params) =>
        new ScanSettings(options.Target, @params)
    }
  };


  public static readonly IEnumerable<object[]> SetterInvalidInput = new List<object[]>
  {
    new object[]
    {
      "Unknown attack param location supplied.",
      (ScanSettings x) =>
        x with
        {
          AttackParamLocations = new List<AttackParamLocation>
          {
            (AttackParamLocation)1024
          }
        }
    },
    new object[]
    {
      "Please provide at least one attack parameter location.",
      (ScanSettings x) =>
        x with
        {
          AttackParamLocations = new List<AttackParamLocation>()
        }
    },
    new object[]
    {
      "Unknown test type supplied.",
      (ScanSettings x) =>
        x with
        {
          Tests = new List<TestType>
          {
            (TestType)1024
          }
        }
    },
    new object[]
    {
      "Please provide at least one test.",
      (ScanSettings x) =>
        x with
        {
          Tests = new List<TestType>()
        }
    },
    new object[]
    {
      "Invalid target connection timeout.",
      (ScanSettings x) =>
        x with
        {
          TargetTimeout = null
        }
    },
    new object[]
    {
      "Invalid target connection timeout.",
      (ScanSettings x) =>
        x with
        {
          TargetTimeout = TimeSpan.FromSeconds(0)
        }
    },
    new object[]
    {
      "Invalid target connection timeout.",
      (ScanSettings x) =>
        x with
        {
          TargetTimeout = TimeSpan.FromSeconds(121)
        }
    },
    new object[]
    {
      "Invalid slow entry point timeout.",
      (ScanSettings x) =>
        x with
        {
          SlowEpTimeout = null
        }
    },
    new object[]
    {
      "Invalid slow entry point timeout.",
      (ScanSettings x) =>
        x with
        {
          SlowEpTimeout = TimeSpan.FromSeconds(99)
        }
    },
    new object[]
    {
      "Invalid pool size.",
      (ScanSettings x) =>
        x with
        {
          PoolSize = null
        }
    },
    new object[]
    {
      "Invalid pool size.",
      (ScanSettings x) =>
        x with
        {
          PoolSize = 0
        }
    },
    new object[]
    {
      "Invalid pool size.",
      (ScanSettings x) =>
        x with
        {
          PoolSize = 51
        }
    },
    new object[]
    {
      "Name must be less than 200 characters.",
      (ScanSettings x) =>
        x with
        {
          Name = null
        }
    },
    new object[]
    {
      "Name must be less than 200 characters.",
      (ScanSettings x) =>
        x with
        {
          Name = " "
        }
    },
    new object[]
    {
      "Name must be less than 200 characters.",
      (ScanSettings x) =>
        x with
        {
          Name = new string('a', 201)
        }
    }
  };

  private readonly TestScanSettingsOptions _testScanSettingsOptions;

  private readonly TestTargetOptions _testTargetOptions;

  public ScanSettingsTests()
  {
    _testTargetOptions = new TestTargetOptions(TargetUrl)
    {
      Body = new StringContent("{}", Encoding.UTF8, "application/json"),
      Headers = new List<KeyValuePair<string, IEnumerable<string>>>(),
      Method = HttpMethod.Get,
      Query = new List<KeyValuePair<string, string>>()
    };

    _testScanSettingsOptions = new TestScanSettingsOptions(new List<TestType>
    {
      TestType.HeaderSecurity
    }, _testTargetOptions)
    {
      RepeaterId = RepeaterId,
      Name = ScanName,
      Smart = false,
      PoolSize = 1,
      TargetTimeout = TimeSpan.FromSeconds(5),
      SlowEpTimeout = TimeSpan.FromSeconds(200),
      SkipStaticParams = false,
      AttackParamLocations = new List<AttackParamLocation>
      {
        AttackParamLocation.Query
      }
    };
  }

  [Fact]
  public void ScanSettings_CreatesInstanceWithDefaultOptions()
  {
    // act
    var result = new ScanSettings(_testTargetOptions, new List<TestType>
    {
      TestType.Csrf
    });

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
      Tests = new List<TestType>
      {
        TestType.Csrf
      },
      AttackParamLocations = new List<AttackParamLocation>
      {
        AttackParamLocation.Body, AttackParamLocation.Query, AttackParamLocation.Fragment
      },
      Target = new
      {
        Url = TargetUrl
      }
    });
  }

  [Fact]
  public void ScanSettings_ScanSettingsOptionsPassed_CreatesInstance()
  {
    // act
    var result = new ScanSettings(_testScanSettingsOptions);

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
      Tests = new List<TestType>
      {
        TestType.HeaderSecurity
      },
      AttackParamLocations = new List<AttackParamLocation>
      {
        AttackParamLocation.Query
      },
      Target = new
      {
        Url = TargetUrl
      }
    });
  }

  [Fact]
  public void ScanSettings_NameIsTooLong_ThrowsError()
  {
    // arrange
    var scanSettingsOptions = _testScanSettingsOptions with
    {
      Name = new string('a', 1 + ScanSettings.MaxNameLength)
    };

    // act
    var act = () => new ScanSettings(scanSettingsOptions);

    // assert
    act.Should().Throw<ArgumentException>()
      .WithMessage($"Name must be less than {ScanSettings.MaxNameLength} characters.");
  }

  [Theory]
  [MemberData(nameof(DefaultNameInput))]
  public void ScanSettings_NameIsNull_CreatesInstanceWithDefaultName(
    string expected, Func<TestScanSettingsOptions, ScanSettings> creatorFunc
  )
  {
    // act
    var result = creatorFunc(_testScanSettingsOptions);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Name = expected
    });
  }

  [Theory]
  [MemberData(nameof(UniqueAttackParamLocationsInput))]
  public void ScanSettings_CreatesInstanceWithUniqueAttackParamLocations(
    Func<TestScanSettingsOptions, IEnumerable<AttackParamLocation>, ScanSettings> creatorFunc)
  {
    // arrange
    var attackParamLocations =
      new List<AttackParamLocation>
      {
        AttackParamLocation.Header, AttackParamLocation.Header
      };

    // act
    var result = creatorFunc(_testScanSettingsOptions, attackParamLocations);

    // assert
    result.Should().BeEquivalentTo(new
    {
      AttackParamLocations = new List<AttackParamLocation>
      {
        AttackParamLocation.Header
      }
    });
  }

  [Theory]
  [MemberData(nameof(UniqueTestTypesInput))]
  public void ScanSettings_CreatesInstanceWithUniqueTests(
    Func<TestScanSettingsOptions, IEnumerable<TestType>, ScanSettings> creatorFunc)
  {
    // arrange
    var tests = new List<TestType>
    {
      TestType.Csrf, TestType.Csrf, TestType.Hrs
    };

    // act
    var result = creatorFunc(_testScanSettingsOptions, tests);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Tests = new List<TestType>
      {
        TestType.Csrf, TestType.Hrs
      }
    });
  }


  [Theory]
  [MemberData(nameof(SetterInvalidInput))]
  public void Setter_GivenInvalidInput_ThrowsError(string expected,
    Func<ScanSettings, ScanSettings> mutatorFunc)
  {
    // arrange
    var scanSettings = new ScanSettings(_testScanSettingsOptions);

    // act
    var act = () => mutatorFunc(scanSettings);

    // assert
    act.Should().Throw<ArgumentException>().WithMessage(expected);
  }
}
