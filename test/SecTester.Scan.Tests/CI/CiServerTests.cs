namespace SecTester.Scan.Tests.CI;

public class CiServerTests
{
  public static IEnumerable<object[]> FromInvalidInput = new List<object[]>
  {
    new object[]
    {
      null!, ""
    },
    new object[]
    {
      string.Empty, ""
    }
  };

  public static IEnumerable<object[]> EqualityInput = new List<object[]>
  {
    new object[]
    {
      () => CiServer.Appveyor == CiServer.Appveyor, true,
    },
    new object[]
    {
      () => CiServer.Appveyor != CiServer.Appveyor, false,
    },
    new object[]
    {
      () => CiServer.Appveyor != CiServer.Bamboo, true,
    },
    new object[]
    {
      () => CiServer.Appveyor == CiServer.Bamboo, false,
    },
    new object[]
    {
      () => CiServer.Appveyor.Equals(CiServer.Appveyor), true,
    },
    new object[]
    {
      () => CiServer.Appveyor.Equals(null!), false,
    },
    new object[]
    {
      () => CiServer.Appveyor.Equals((object)CiServer.Appveyor), true,
    },
    new object[]
    {
      () => CiServer.Appveyor.Equals(null as object), false,
    },
    new object[]
    {
      () => CiServer.From("constant1", "") ==  CiServer.From("CONSTANT1", ""), true,
    }
  };

  [Theory]
  [MemberData(nameof(EqualityInput))]
  public void EqualityExpression_ReturnsExpected(Func<bool> actorFunc, bool expected)
  {
    // act
    var result = actorFunc();

    // assert
    result.Should().Be(expected);
  }

  [Fact]
  public void ToString_ReturnsConstant()
  {
    // assert
    CiServer.CodeBuild.ToString().Should().Be(CiServer.CodeBuild.Constant);
  }

  [Fact]
  public void From_ReturnsPredefinedInstance()
  {
    // act
    var result = CiServer.From(CiServer.Bamboo.Constant, "NAME");

    // assert
    result.Should().BeSameAs(CiServer.Bamboo);
  }

  [Fact]
  public void From_CreatesInstance()
  {
    // act
    var result = CiServer.From("CONSTANT", "NAME");

    // assert
    result.Name.Should().Be("NAME");
    result.Constant.Should().Be("CONSTANT");
  }

  [Theory]
  [MemberData(nameof(FromInvalidInput))]
  public void From_GivenInvalidInput_ThrowsError(string constantInput, string nameInput)
  {
    // act
    var act = () => CiServer.From(constantInput, nameInput);

    // assert
    act.Should().Throw<ArgumentException>().WithMessage("Constant value must not be empty.");
  }

  [Fact]
  public void From_GivenConstant_ReturnsInstanceWithSameHashCode()
  {
    // arrange
    var ciServer = CiServer.From("constant1", "");

    // act
    var result = CiServer.From(ciServer.ToString().ToUpperInvariant(), "");

    // assert
    result.GetHashCode().Should().Be(ciServer.GetHashCode());
  }
}
