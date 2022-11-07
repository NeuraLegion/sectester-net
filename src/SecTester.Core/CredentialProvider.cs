using System.Threading.Tasks;

namespace SecTester.Core;

public interface CredentialProvider
{
  Task<Credentials?> Get();
}
