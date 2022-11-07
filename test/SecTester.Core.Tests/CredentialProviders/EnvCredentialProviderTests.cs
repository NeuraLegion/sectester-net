using SecTester.Core.CredentialProviders;

namespace SecTester.Core.Tests.CredentialProviders;

public class EnvCredentialProviderTests
{
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
      const string token = "0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk";
      Environment.SetEnvironmentVariable(EnvCredentialProvider.BrightToken, token);
      var envCredentialProvider = new EnvCredentialProvider();

      // act
      var result = await envCredentialProvider.Get();

      // assert
      result.Should().BeEquivalentTo(new { Token = token });
    }
    finally
    {
      Environment.SetEnvironmentVariable(EnvCredentialProvider.BrightToken, oldValue);
    }

  }
}
