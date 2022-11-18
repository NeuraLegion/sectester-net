using System.Text.Json;
using SecTester.Core.Utils;

namespace SecTester.Core.Json;

class SnakeCaseNamingPolicy : JsonNamingPolicy
{
  public static SnakeCaseNamingPolicy Instance { get; } = new SnakeCaseNamingPolicy();

  public override string ConvertName(string name)
  {
    return name.ToSnakeCase();
  }
}
