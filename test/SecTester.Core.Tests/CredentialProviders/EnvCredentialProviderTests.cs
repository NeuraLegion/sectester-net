namespace SecTester.Core.Tests.CredentialProviders;

public class EnvCredentialProviderTests
{
  private const string Token = "0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk";

  [Fact]
  public async Task Get_EnvVariableIsNotProvided_ReturnNull()
  {
    // arrange
    var envCredentialProvider = new EnvCredentialProvider();

    // act
    var result = await envCredentialProvider.Get();

    // assert
    result.Should().BeNull();
  }

  [Fact]
  public async Task Get_GivenEnvVariable_ReturnCredentials()
  {
    var oldValue = Environment.GetEnvironmentVariable(EnvCredentialProvider.BrightToken);

    try
    {
      // arrange
      Environment.SetEnvironmentVariable(EnvCredentialProvider.BrightToken, Token);
      var envCredentialProvider = new EnvCredentialProvider();

      // act
      var result = await envCredentialProvider.Get();

      // assert
      result.Should().BeEquivalentTo(new { Token });
    }
    finally
    {
      Environment.SetEnvironmentVariable(EnvCredentialProvider.BrightToken, oldValue);
    }

  }
}
