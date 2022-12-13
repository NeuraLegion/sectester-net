namespace SecTester.Scan.Tests.CI;

public class CiServerTests
{
  public static readonly IEnumerable<object[]> ConstructorInvalidInput = new List<object[]>
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

  public static readonly IEnumerable<object[]> EqualityValidInput = new List<object[]>
  {
    new object[]
    {
      CiServer.Appveyor, CiServer.Appveyor
    },
    new object[]
    {
      new CiServer("id1", ""), new CiServer("ID1", "")
    }
  };

  public static readonly IEnumerable<object[]> EqualityInvalidInput = new List<object[]>
  {
    new object[]
    {
      CiServer.Appveyor, CiServer.Bamboo
    },
    new object[]
    {
      CiServer.Appveyor, null as CiServer
    }
  };

  [Theory]
  [MemberData(nameof(EqualityValidInput))]
  public void ObjectEquals_ReturnsTrue(CiServer inputLeft, CiServer inputRight)
  {
    // act
    var result = inputLeft.Equals(inputRight as object);

    // assert
    result.Should().BeTrue();
  }

  [Theory]
  [MemberData(nameof(EqualityInvalidInput))]
  public void ObjectEquals_ReturnsFalse(CiServer inputLeft, CiServer inputRight)
  {
    // act
    var result = inputLeft.Equals(inputRight as object);

    // assert
    result.Should().BeFalse();
  }

  [Theory]
  [MemberData(nameof(EqualityValidInput))]
  public void IEquitableEquals_ReturnsTrue(CiServer inputLeft, CiServer inputRight)
  {
    // act
    var result = inputLeft.Equals(inputRight);

    // assert
    result.Should().BeTrue();
  }

  [Theory]
  [MemberData(nameof(EqualityInvalidInput))]
  public void IEquitableEquals_ReturnsFalse(CiServer inputLeft, CiServer inputRight)
  {
    // act
    var result = inputLeft.Equals(inputRight);

    // assert
    result.Should().BeFalse();
  }

  [Theory]
  [MemberData(nameof(EqualityValidInput))]
  public void EqualityOperator_ReturnsTrue(CiServer inputLeft, CiServer inputRight)
  {
    // act
    var result = inputLeft == inputRight;

    // assert
    result.Should().BeTrue();
  }

  [Theory]
  [MemberData(nameof(EqualityInvalidInput))]
  public void EqualityOperator_ReturnsFalse(CiServer inputLeft, CiServer inputRight)
  {
    // act
    var result = inputLeft == inputRight;

    // assert
    result.Should().BeFalse();
  }

  [Theory]
  [MemberData(nameof(EqualityInvalidInput))]
  public void InequalityOperator_ReturnsTrue(CiServer inputLeft, CiServer inputRight)
  {
    // act
    var result = inputLeft != inputRight;

    // assert
    result.Should().BeTrue();
  }

  [Theory]
  [MemberData(nameof(EqualityValidInput))]
  public void InequalityOperator_ReturnsFalse(CiServer inputLeft, CiServer inputRight)
  {
    // act
    var result = inputLeft != inputRight;

    // assert
    result.Should().BeFalse();
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
    act.Should().Throw<ArgumentNullException>().WithMessage("*id*");
  }

  [Fact]
  public void GetHashCode_WithDifferentCaseId_ReturnsSameHashCode()
  {
    // arrange
    var hashcode = new CiServer("id1", "").GetHashCode();

    // act
    var result = new CiServer("ID1", "").GetHashCode();

    // assert
    result.Should().Be(hashcode);
  }

  [Fact]
  public void From_GivenExistingId_ReturnsExistingInstance()
  {
    // act
    var result = CiServer.From(CiServer.Appveyor.Id);

    // assert
    result.Should().BeSameAs(CiServer.Appveyor);
  }
}
