using System;
using System.Collections;
using System.Linq;
using System.Text.Json;

namespace SecTester.Scan.CI;

internal class DefaultCiDiscovery : CiDiscovery
{
  private const string VendorsResource = "SecTester.Scan.CI.vendors.json";

  public CiServer? Server { get; }
  public bool IsCi => Server != null;
  public bool IsPr { get; }

  public DefaultCiDiscovery(IDictionary? env = default)
  {
    env ??= Environment.GetEnvironmentVariables();

    var vendors = JsonSerializer.Deserialize<Vendor[]>(
      ResourceUtils.GetEmbeddedResourceContent<DefaultCiDiscovery>(VendorsResource),
      new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

    if (vendors is null)
    {
      return;
    }

    var matcher = new VendorMatcher(env);

    var vendor = vendors.FirstOrDefault(x => matcher.MatchEnv(x.Env));

    if (vendor is null)
    {
      return;
    }

    Server = CiServer.GetAll()
               .FirstOrDefault(x => vendor.Constant.Equals(x.Id, StringComparison.OrdinalIgnoreCase))
             ?? new CiServer(vendor.Constant, vendor.Name);

    IsPr = matcher.MatchPr(vendor.Pr);
  }
}
