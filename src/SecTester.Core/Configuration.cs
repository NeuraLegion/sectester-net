using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SecTester.Core
{
  public class Configuration
  {
    private static readonly Regex SchemaPattern = new Regex(@"^.+:\/\/");
    private static readonly Regex HostnameNormalizationPattern = new Regex(@"^(?!(?:\w+:)?\/\/)|^\/\/");
    private static readonly string[] LoopbackAddresses = { "localhost", "127.0.0.1" };

    public string Bus { get; private set; }

    public string Api { get; private set; }

    // TODO: provide a more convenient way of setting these properties
    public readonly string Name = "sectester-net";
    public readonly string Version = "0.0.1";
    public readonly string RepeaterVersion = "9.0.0";

    public Configuration(string hostname)
    {
      hostname = hostname?.Trim();

      if (string.IsNullOrEmpty(hostname))
      {
        throw new Exception("Please provide 'hostname' option.");
      }

      ResolveUrls(hostname);
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
        throw new Exception("Please make sure that you pass correct 'hostname' option.");
      }
    }

    private void ResolveUrls(Uri uri)
    {
      var host = uri.Host;
      if (LoopbackAddresses.Any(address => address == host))
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
      return !SchemaPattern.IsMatch(hostname)
        ? HostnameNormalizationPattern.Replace(
          hostname,
          "https://"
        )
        : hostname;
    }
  }
}
