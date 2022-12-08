using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace SecTester.Scan.CI;

internal class DefaultCiDiscovery : CiDiscovery
{
  public CiServer? Server { get; }

  public bool IsCi => Server != null;

  public bool IsPr { get; }

  public DefaultCiDiscovery(IDictionary? env = default)
  {
    env ??= Environment.GetEnvironmentVariables();

    var vendors = JsonSerializer.Deserialize<Vendor[]>(
      ResourceUtils.GetEmbeddedResourceContent<DefaultCiDiscovery>("SecTester.Scan.CI.vendors.json"),
      new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

    if (vendors is null)
    {
      return;
    }

    var matcher = new VendorMatcher(env);

    var vendor = vendors.FirstOrDefault(x => matcher.MatchEnvElement(x.Env));

    if (vendor is null)
    {
      return;
    }

    Server = typeof(CiServer)
      .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
      .Where(x => x.Name.Equals(vendor.Constant, StringComparison.OrdinalIgnoreCase))
      .Select(x => x.GetValue(null))
      .Cast<CiServer>()
      .FirstOrDefault() ?? new CiServer(vendor.Name);

    IsPr = matcher.MatchPrElement(vendor.Pr);
  }
}
