namespace SecTester.Core.Tests.Logger;

public class AnsiCodeColorTests
{
  private const string White = "\x1B[1m\x1B[37m";

  public static readonly IEnumerable<object[]> ConstructorInvalidInput = new List<object[]>
  {
    new object[]
    {
      null!
    },
    new object[]
    {
      string.Empty
    }
  };

  public static readonly IEnumerable<object[]> EqualityValidInput = new List<object[]>
  {
    new object[]
    {
      AnsiCodeColor.Cyan, AnsiCodeColor.Cyan
    },
    new object[]
    {
      new AnsiCodeColor("color1"), new AnsiCodeColor("COLOR1")
    }
  };

  public static readonly IEnumerable<object[]> EqualityInvalidInput = new List<object[]>
  {
    new object[]
    {
      AnsiCodeColor.White, AnsiCodeColor.Yellow
    },
    new object[]
    {
      AnsiCodeColor.White, null!
    }
  };

  [Theory]
  [MemberData(nameof(EqualityValidInput))]
  public void ObjectEquals_ReturnsTrue(AnsiCodeColor inputLeft, AnsiCodeColor inputRight)
  {
    // act
    var result = inputLeft.Equals(inputRight as object);

    // assert
    result.Should().BeTrue();
  }

  [Theory]
  [MemberData(nameof(EqualityInvalidInput))]
  public void ObjectEquals_ReturnsFalse(AnsiCodeColor inputLeft, AnsiCodeColor inputRight)
  {
    // act
    var result = inputLeft.Equals(inputRight as object);

    // assert
    result.Should().BeFalse();
  }

  [Theory]
  [MemberData(nameof(EqualityValidInput))]
  public void IEquitableEquals_ReturnsTrue(AnsiCodeColor inputLeft, AnsiCodeColor inputRight)
  {
    // act
    var result = inputLeft.Equals(inputRight);

    // assert
    result.Should().BeTrue();
  }

  [Theory]
  [MemberData(nameof(EqualityInvalidInput))]
  public void IEquitableEquals_ReturnsFalse(AnsiCodeColor inputLeft, AnsiCodeColor inputRight)
  {
    // act
    var result = inputLeft.Equals(inputRight);

    // assert
    result.Should().BeFalse();
  }

  [Theory]
  [MemberData(nameof(EqualityValidInput))]
  public void EqualityOperator_ReturnsTrue(AnsiCodeColor inputLeft, AnsiCodeColor inputRight)
  {
    // act
    var result = inputLeft == inputRight;

    // assert
    result.Should().BeTrue();
  }

  [Theory]
  [MemberData(nameof(EqualityInvalidInput))]
  public void EqualityOperator_ReturnsFalse(AnsiCodeColor inputLeft, AnsiCodeColor inputRight)
  {
    // act
    var result = inputLeft == inputRight;

    // assert
    result.Should().BeFalse();
  }

  [Theory]
  [MemberData(nameof(EqualityInvalidInput))]
  public void InequalityOperator_ReturnsTrue(AnsiCodeColor inputLeft, AnsiCodeColor inputRight)
  {
    // act
    var result = inputLeft != inputRight;

    // assert
    result.Should().BeTrue();
  }

  [Theory]
  [MemberData(nameof(EqualityValidInput))]
  public void InequalityOperator_ReturnsFalse(AnsiCodeColor inputLeft, AnsiCodeColor inputRight)
  {
    // act
    var result = inputLeft != inputRight;

    // assert
    result.Should().BeFalse();
  }

  [Fact]
  public void ToString_ReturnsAnsiCode()
  {
    // assert
    AnsiCodeColor.White.ToString().Should().Be(White);
  }

  [Fact]
  public void ImplicitToStringOperator_ReturnsColor()
  {
    // act
    string value = AnsiCodeColor.White;

    // assert
    value.Should().Be(White);
  }

  [Fact]
  public void Constructor_CreatesInstance()
  {
    // act
    var result = new AnsiCodeColor(White);

    // assert
    result.ToString().Should().Be(White);
  }

  [Theory]
  [MemberData(nameof(ConstructorInvalidInput))]
  public void Constructor_GivenInvalidInput_ThrowsError(string input)
  {
    // act
    var act = () => new AnsiCodeColor(input);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*color*");
  }

  [Fact]
  public void GetHashCode_WithDifferentCaseId_ReturnsSameHashCode()
  {
    // arrange
    var hashcode = new AnsiCodeColor("color1").GetHashCode();

    // act
    var result = new AnsiCodeColor("COLOR1").GetHashCode();

    // assert
    result.Should().Be(hashcode);
  }
}
