using System;
using System.Collections;
using System.Linq;
using System.Text.Json;

namespace SecTester.Scan.CI;

internal class VendorMatcher
{
  private readonly IDictionary _environment;

  public VendorMatcher(IDictionary environment)
  {
    _environment = environment;
  }

  public bool MatchEnv(JsonElement element)
  {
    return element.ValueKind switch
    {
      JsonValueKind.String => HasProperty(element),
      JsonValueKind.Object => MatchObject(element),
      JsonValueKind.Array => element.EnumerateArray().All(HasProperty),
      _ => false
    };
  }

  public bool MatchPr(JsonElement element)
  {
    return element.ValueKind switch
    {
      JsonValueKind.String => HasProperty(element),
      JsonValueKind.Object => NotEqual(element) || MatchObject(element),
      _ => false
    };
  }

  private bool MatchObject(JsonElement element)
  {
    return Any(element) || Includes(element) || HasOwnProperties(element);
  }

  private bool HasProperty(JsonElement element)
  {
    return element.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(element.GetString()) && _environment.Contains(element.GetString()!);
  }

  private bool HasOwnProperties(JsonElement element)
  {
    return element.EnumerateObject().All(x =>
      x.Value.ValueKind == JsonValueKind.String && _environment.Contains(x.Name) &&
      x.Value.ValueEquals(_environment[x.Name]?.ToString()));
  }

  private bool Any(JsonElement element)
  {
    var any = element.EnumerateObject().FirstOrDefault(x => x.NameEquals("any"));

    return any.Value.ValueKind == JsonValueKind.Array && any.Value.EnumerateArray().Any(HasProperty);
  }

  private bool Includes(JsonElement element)
  {
    return ApplyPredicate(element, "includes", (envVarValue, includesValue) =>
      envVarValue?.Contains(includesValue) ?? false
    );
  }

  private bool NotEqual(JsonElement element)
  {
    return ApplyPredicate(element, "ne", (envVarValue, neValue) =>
      neValue == "false" && !string.IsNullOrEmpty(envVarValue)
    );
  }

  private string? GetEnvValue(JsonElement element)
  {
    var env = GetPropertyValue(element, "env");

    if (string.IsNullOrEmpty(env) || !_environment.Contains(env))
    {
      return null;
    }

    return _environment[env]?.ToString();
  }

  private bool ApplyPredicate(JsonElement element, string propertyName, Func<string?, string, bool> predicate)
  {
    var env = GetEnvValue(element);
    var property = GetPropertyValue(element, propertyName);

    if (string.IsNullOrEmpty(env) || string.IsNullOrEmpty(property))
    {
      return false;
    }

    return predicate(env, property!);
  }

  private static string? GetPropertyValue(JsonElement element, string name)
  {
    var property = element.EnumerateObject().FirstOrDefault(x => x.NameEquals(name));
    return property.Value.ValueKind == JsonValueKind.String ? property.Value.GetString() : default;
  }
}
