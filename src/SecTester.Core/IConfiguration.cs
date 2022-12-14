using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SecTester.Core;

public interface IConfiguration
{
  string Hostname { get; }
  Credentials? Credentials { get; }
  IEnumerable<ICredentialProvider>? CredentialProviders { get; }
  LogLevel LogLevel { get; }
}
