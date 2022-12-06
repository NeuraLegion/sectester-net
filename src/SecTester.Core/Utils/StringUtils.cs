using System.Linq;
using System.Text.RegularExpressions;

namespace SecTester.Core.Utils;

public static class StringUtils
{
  private static readonly Regex CamelCaseRegex = new(@"([a-z0-9])([A-Z])");
  private static readonly Regex PascalCaseRegex = new(@"([A-Z])([A-Z][a-z])");

  private static readonly Regex[] SupportedCaseRegexes =
  {
    PascalCaseRegex, CamelCaseRegex
  };

  public static string? ToSnakeCase(this string? value)
  {
    return string.IsNullOrEmpty(value)
      ? value
      : SupportedCaseRegexes.Aggregate(value!,
          (input, regex) => regex.Replace(input, "$1_$2"))
        .ToLowerInvariant();
  }

  public static string? Truncate(this string? value, int n)
  {
    return string.IsNullOrEmpty(value) || n < 0 || value!.Length <= n
      ? value
      : $"{value.Substring(0, n)}…";
  }
}
