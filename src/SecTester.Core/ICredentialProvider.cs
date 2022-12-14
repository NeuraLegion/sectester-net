using System.Threading.Tasks;

namespace SecTester.Core;

public interface ICredentialProvider
{
  Task<Credentials?> Get();
}
