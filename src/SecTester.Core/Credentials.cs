using System;
using System.Text.RegularExpressions;

namespace SecTester.Core
{
  public class Credentials
  {
    private readonly Regex _tokenValidationRegexp = new(@"^[A-Za-z0-9+/=]{7}\.nex[apr]\.[A-Za-z0-9+/=]{32}$");
    public string Token { get; }

    public Credentials(string token)
    {
      Token = token ?? throw new ArgumentNullException(nameof(token), "Provide an API key.");

      if (!_tokenValidationRegexp.IsMatch(token))
      {
        throw new Exception("Unable to recognize the API key.");
      }
    }
  }
}
