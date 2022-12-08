using System;
using System.Threading.Tasks;

namespace SecTester.Core.CredentialProviders;

public class EnvCredentialProvider : CredentialProvider
{
  public const string BrightToken = "BRIGHT_TOKEN";

  public Task<Credentials?> Get()
  {
    var token = Environment.GetEnvironmentVariable(BrightToken);

    return Task.FromResult(!string.IsNullOrWhiteSpace(token) ? new Credentials(token) : null);
  }
}
