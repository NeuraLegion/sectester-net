using System.Text.Json;

namespace SecTester.Scan.CI;

internal record Vendor(string Name, string Constant)
{
  public JsonElement Env { get; init; }
  public JsonElement Pr { get; init; }
}
