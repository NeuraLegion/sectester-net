using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SecTester.Core.CredentialProviders;
using SecTester.Core.Logger;
using SecTester.Core.Utils;

namespace SecTester.Core
{
  public class Configuration
  {
    private readonly Regex _schemaRegex = new(@"^.+:\/\/");
    private readonly Regex _hostnameNormalizationRegex = new(@"^(?!(?:\w+:)?\/\/)|^\/\/");
    private readonly string[] _loopbackAddresses = { "localhost", "127.0.0.1" };
    private readonly List<CredentialProvider> _credentialProviders;

    public string Bus { get; private set; }

    public string Api { get; private set; }

    public Credentials? Credentials { get; private set; }
    
    public LogLevel LogLevel { get; private set; }

    public IReadOnlyCollection<CredentialProvider> CredentialProviders => _credentialProviders.AsReadOnly();

    // TODO: provide a more convenient way of setting these properties
    public string Name { get; } = "sectester-net";
    public string Version { get; } = "0.0.1";
    public string RepeaterVersion { get; } = "9.0.0";

    public Configuration(string? hostname, Credentials? credentials = null, List<CredentialProvider>? credentialProviders = null, LogLevel logLevel = LogLevel.Error)
    {
      LogLevel = logLevel;
      
      credentialProviders ??= new List<CredentialProvider> { new EnvCredentialProvider() };
      hostname = hostname?.Trim();
      hostname = hostname ?? throw new ArgumentNullException(nameof(hostname), "Please provide 'hostname' option.");

      ResolveUrls(hostname);

      if (credentials == null && (credentialProviders == null || !credentialProviders.Any()))
      {
        throw new InvalidOperationException(
          $"Please provide either '{nameof(credentials)}' or '{nameof(credentialProviders)}'"
        );
      }

      Credentials = credentials;
      _credentialProviders = credentialProviders;
    }

    public async Task LoadCredentials()
    {
      if (Credentials == null)
      {
        var chain = _credentialProviders.ConvertAll(provider =>
          provider.Get()
        );
        var credentials = await TaskUtils.First(chain, x => x != null);
        Credentials = credentials ?? throw new InvalidOperationException("Could not load credentials from any providers");
      }
    }

    private void ResolveUrls(string hostname)
    {
      try
      {
        var uri = new Uri(AddSchema(hostname));
        ResolveUrls(uri);
      }
      catch
      {
        throw new InvalidOperationException("Please make sure that you pass correct 'hostname' option.");
      }
    }

    private void ResolveUrls(Uri uri)
    {
      var host = uri.Host;
      if (_loopbackAddresses.Any(address => address == host))
      {
        Bus = $"amqp://{host}:5672";
        Api = $"http://{host}:8000";
      }
      else
      {
        Bus = $"amqps://amq.{host}:5672";
        Api = $"https://{host}";
      }
    }

    private string AddSchema(string hostname)
    {
      return !_schemaRegex.IsMatch(hostname)
        ? _hostnameNormalizationRegex.Replace(
          hostname,
          "https://"
        )
        : hostname;
    }
  }
}
