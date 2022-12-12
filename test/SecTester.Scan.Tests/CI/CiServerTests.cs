namespace SecTester.Scan.Tests.CI;

public class CiServerTests
{
  public static IEnumerable<object[]> ConstructorInvalidInput = new List<object[]>
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
      () => new CiServer("id1", "") == new CiServer("ID1", ""), true,
    },
    new object[]
    {
      () => new CiServer("id1", "").GetHashCode() == new CiServer("ID1", "").GetHashCode(), true,
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
  public void ToString_ReturnsName()
  {
    // assert
    CiServer.CodeBuild.ToString().Should().Be(CiServer.CodeBuild.Name);
  }

  [Fact]
  public void Constructor_CreatesInstance()
  {
    // act
    var result = new CiServer("ID", "NAME");

    // assert
    result.Name.Should().Be("NAME");
    result.Id.Should().Be("ID");
  }

  [Theory]
  [MemberData(nameof(ConstructorInvalidInput))]
  public void Constructor_GivenInvalidInput_ThrowsError(string idInput, string nameInput)
  {
    // act
    var act = () => new CiServer(idInput, nameInput);

    // assert
    act.Should().Throw<ArgumentException>().WithMessage("Id must not be empty.");
  }
}
