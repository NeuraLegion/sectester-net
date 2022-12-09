using System.Collections;
using System.Linq;
using System.Text.Json;

namespace SecTester.Scan.CI;

internal class VendorMatcher
{
  private readonly IDictionary _env;

  public VendorMatcher(IDictionary env)
  {
    _env = env;
  }

  public bool MatchEnv(JsonElement element)
  {
    return element.ValueKind switch
    {
      JsonValueKind.String => CheckStringValue(element),
      JsonValueKind.Object => Any(element) || Includes(element) || HasOwnProperties(element),
      JsonValueKind.Array => element.EnumerateArray().All(CheckStringValue),
      _ => false
    };
  }

  public bool MatchPr(JsonElement element)
  {
    return element.ValueKind switch
    {
      JsonValueKind.String => CheckStringValue(element),
      JsonValueKind.Object => NotEqual(element) || MatchEnv(element),
      _ => false
    };
  }

  private bool CheckStringValue(JsonElement element)
  {
    return element.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(element.GetString()) && _env.Contains(element.GetString()!);
  }

  private bool HasOwnProperties(JsonElement element)
  {
    return element.EnumerateObject().All(x =>
      x.Value.ValueKind == JsonValueKind.String && _env.Contains(x.Name) &&
      x.Value.ValueEquals(_env[x.Name]?.ToString()));
  }

  private bool Any(JsonElement element)
  {
    var any = element.EnumerateObject().FirstOrDefault(x => x.NameEquals("any"));

    return any.Value.ValueKind == JsonValueKind.Array && any.Value.EnumerateArray().Any(CheckStringValue);
  }

  private bool Includes(JsonElement element)
  {
    var env = GetPropertyValue(element, "env");
    var includes = GetPropertyValue(element, "includes");

    if (string.IsNullOrEmpty(env) || string.IsNullOrEmpty(includes) || !_env.Contains(env))
    {
      return false;
    }

    var envVarValue = _env[env]?.ToString();
    return envVarValue?.Contains(includes) ?? false;
  }

  private bool NotEqual(JsonElement element)
  {
    var env = GetPropertyValue(element, "env");
    var ne = GetPropertyValue(element, "ne");

    if (string.IsNullOrEmpty(env) || string.IsNullOrEmpty(ne) || !_env.Contains(env) || ne != "false")
    {
      return false;
    }

    var envVarValue = _env[env]?.ToString();
    return !string.IsNullOrEmpty(envVarValue);
  }

  private static string? GetPropertyValue(JsonElement element, string name)
  {
    var property = element.EnumerateObject().FirstOrDefault(x => x.NameEquals(name));
    return property.Value.ValueKind == JsonValueKind.String ? property.Value.GetString() : default;
  }
}
