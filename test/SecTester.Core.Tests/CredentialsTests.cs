namespace SecTester.Core.Tests;

public class CredentialsTests
{
  public static IEnumerable<object[]> Tokens => new List<object[]>
  {
    new object[] { "weobbz5.nexa.vennegtzr2h7urpxgtksetz2kwppdgj0" },
    new object[] { "w0iikzf.nexp.aeish9lhiag7ledmsdwpwcbilagupc3r" },
    new object[] { "0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk" },
  };

  [Fact]
  public void Credentials_TokenIsInvalid_ThrowError()
  {
    // arrange
    const string token = "qwerty";

    // act
    var act = () => new Credentials(token);

    // assert
    act.Should().Throw<Exception>();
  }

  [Fact]
  public void Credentials_TokenIsNotDefined_ThrowError()
  {
    // arrange
    const string token = "";

    // act
    var act = () => new Credentials(token);

    // assert
    act.Should().Throw<Exception>();
  }

  [Theory]
  [MemberData(nameof(Tokens))]
  public void Credentials_ValidToken_Set(string input)
  {
    // act
    var credentials = new Credentials(input);

    // assert
    credentials.Should().BeEquivalentTo(new { Token = input });
  }
}
