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

  public bool MatchEnvElement(JsonElement element)
  {
    return element.ValueKind switch
    {
      JsonValueKind.String => element.GetString() is not null && _env.Contains(element.GetString()!),
      JsonValueKind.Object => MatchInnerAny(element) || MatchInnerEnvIncludes(element) ||
                              MatchOwnProperties(element),
      JsonValueKind.Array => element.EnumerateArray()
        .All(x => x.ValueKind == JsonValueKind.String && x.GetString() is not null && _env.Contains(x.GetString()!)),
      _ => false
    };
  }


  public bool MatchPrElement(JsonElement element)
  {
    return element.ValueKind switch
    {
      JsonValueKind.String => element.GetString() is not null && _env.Contains(element.GetString()!),
      JsonValueKind.Object => MatchInnerEnvNe(element) || MatchEnvElement(element),
      _ => false
    };
  }

  private bool MatchOwnProperties(JsonElement element)
  {
    return element.EnumerateObject().All(x =>
      x.Value.ValueKind == JsonValueKind.String && _env.Contains(x.Name) &&
      x.Value.ValueEquals(_env[x.Name]?.ToString()));
  }

  private bool MatchInnerAny(JsonElement element)
  {
    var any = element.EnumerateObject().FirstOrDefault(x => x.NameEquals("any"));

    return any.Value.ValueKind == JsonValueKind.Array && any.Value.EnumerateArray()
      .Any(x => x.ValueKind == JsonValueKind.String && x.GetString() is not null && _env.Contains(x.GetString()!));
  }

  private bool MatchInnerEnvIncludes(JsonElement element)
  {
    var env = GetInnerPropertyValue(element, "env");
    var includes = GetInnerPropertyValue(element, "includes");

    if (env is null || includes is null || !_env.Contains(env))
    {
      return false;
    }

    var envVarValue = _env[env]?.ToString();
    return envVarValue?.Contains(includes) ?? false;
  }

  private bool MatchInnerEnvNe(JsonElement element)
  {
    var env = GetInnerPropertyValue(element, "env");
    var ne = GetInnerPropertyValue(element, "ne");

    if (env is null || ne is null || !_env.Contains(env) || ne != "false")
    {
      return false;
    }

    var envVarValue = _env[env]?.ToString();
    return !string.IsNullOrEmpty(envVarValue);
  }

  private static string? GetInnerPropertyValue(JsonElement element, string name)
  {
    var property = element.EnumerateObject().FirstOrDefault(x => x.NameEquals(name));
    return property.Value.ValueKind == JsonValueKind.String ? property.Value.GetString() : default;
  }
}
