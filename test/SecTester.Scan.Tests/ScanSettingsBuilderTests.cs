namespace SecTester.Scan.Tests;

public class ScanSettingsBuilderTests
{
  private const string RepeaterId = "g5MvgM74sweGcK1U6hvs76";
  private const string Url = "https://example.com";
  private const string DefaultName = "GET example.com";
  private const int PoolSize = 50;

  public static readonly IEnumerable<object[]> Names = new List<object[]>
  {
    new object[] { null!, DefaultName },
    new object[] { "", DefaultName },
    new object[] { "A", "A" }
  };
  private readonly IEnumerable<AttackParamLocation> _attackParamLocations = new List<AttackParamLocation>
  {
    AttackParamLocation.ArtificalFragment, AttackParamLocation.ArtificalQuery
  };
  private readonly ScanSettingsBuilder _sut = new();
  private readonly Target _target = new(Url);
  private readonly IEnumerable<TestType> _tests = new List<TestType> { TestType.HeaderSecurity };

  private readonly TimeSpan _timeout = TimeSpan.FromSeconds(100);

  [Fact]
  public void WithTarget_SetsTargetProperty()
  {
    // arrange
    _sut.WithTests(_tests);

    // act
    var result = _sut.WithTarget(_target);

    // assert
    result.Should().Be(_sut);
    result.Build().Should().BeEquivalentTo(new
    {
      Target = _target
    });
  }

  [Theory]
  [MemberData(nameof(Names))]
  public void WithName_SetsNameProperty(string input, string expected)
  {
    // arrange
    _sut.WithTarget(_target).WithTests(_tests);

    // act
    var result = _sut.WithName(input);

    // assert
    result.Should().Be(_sut);
    result.Build().Should().BeEquivalentTo(new
    {
      Name = expected
    });
  }

  [Fact]
  public void WithRepeater_SetsRepeaterIdProperty()
  {
    // arrange
    _sut.WithTarget(_target).WithTests(_tests);

    // act
    var result = _sut.WithRepeater(RepeaterId);

    // assert
    result.Should().Be(_sut);
    result.Build().Should().BeEquivalentTo(new
    {
      RepeaterId
    });
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void Smart_SetsSmartProperty(bool input)
  {
    // arrange
    _sut.WithTarget(_target).WithTests(_tests);

    // act
    var result = _sut.Smart(input);

    // assert
    result.Should().Be(_sut);
    result.Build().Should().BeEquivalentTo(new
    {
      Smart = input
    });
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void SkipStaticParams_SetsSkipStaticParamsProperty(bool input)
  {
    // arrange
    _sut.WithTarget(_target).WithTests(_tests);

    // act
    var result = _sut.SkipStaticParams(input);

    // assert
    result.Should().Be(_sut);
    result.Build().Should().BeEquivalentTo(new
    {
      SkipStaticParams = input
    });
  }

  [Fact]
  public void WithPoolSize_SetsPoolSizeProperty()
  {
    // arrange
    _sut.WithTarget(_target).WithTests(_tests);

    // act
    var result = _sut.WithPoolSize(PoolSize);

    // assert
    result.Should().Be(_sut);
    result.Build().Should().BeEquivalentTo(new
    {
      PoolSize
    });
  }

  [Fact]
  public void WithTargetTimeout_SetsTargetTimeoutProperty()
  {
    // arrange
    _sut.WithTarget(_target).WithTests(_tests);

    // act
    var result = _sut.WithTargetTimeout(_timeout);

    // assert
    result.Should().Be(_sut);
    result.Build().Should().BeEquivalentTo(new
    {
      TargetTimeout = _timeout
    });
  }

  [Fact]
  public void WithSlowEpTimeout_SetsSlowEpTimeoutProperty()
  {
    // arrange
    _sut.WithTarget(_target).WithTests(_tests);

    // act
    var result = _sut.WithSlowEpTimeout(_timeout);

    // assert
    result.Should().Be(_sut);
    result.Build().Should().BeEquivalentTo(new
    {
      SlowEpTimeout = _timeout
    });
  }

  [Fact]
  public void WithAttackParamLocations_SetsAttackParamLocationsProperty()
  {
    // arrange
    _sut.WithTarget(_target).WithTests(_tests);

    // act
    var result = _sut.WithAttackParamLocations(_attackParamLocations);

    // assert
    result.Should().Be(_sut);
    result.Build().Should().BeEquivalentTo(new
    {
      AttackParamLocations = _attackParamLocations
    });
  }

  [Fact]
  public void Build_ThrowsExceptionWhenCreatingScanSettingsWithMissingTarget()
  {
    // act
    var act = () => _sut.Build();

    // assert
    act.Should().Throw<Exception>()
      .WithMessage($"You have to provide a target by calling the {nameof(ScanSettingsBuilder.WithTarget)} method.");
  }

  [Fact]
  public void Build_WithDefaultsValues_ReturnsValidScanSettings()
  {
    // arrange
    _sut.WithTarget(_target).WithTests(_tests);

    // act
    var result = _sut.Build();

    // assert
    result.Should().BeEquivalentTo(new
    {
      Tests = _tests,
      Target = _target,
      Smart = true,
      SkipStaticParams = true,
      Name = DefaultName,
      PoolSize = 10,
      SlowEpTimeout = TimeSpan.FromMilliseconds(1000),
      TargetTimeout = TimeSpan.FromMinutes(5),
      AttackParamLocations = new List<AttackParamLocation>
      {
        AttackParamLocation.Body, AttackParamLocation.Query, AttackParamLocation.Fragment
      }
    });
  }
}




