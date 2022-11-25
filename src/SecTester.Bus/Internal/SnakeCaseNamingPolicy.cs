using System.Text.Json;
using SecTester.Core.Utils;

namespace SecTester.Bus.Internal;

internal class SnakeCaseNamingPolicy : JsonNamingPolicy
{
  public static JsonNamingPolicy Instance { get; } = new SnakeCaseNamingPolicy();

  public override string ConvertName(string name)
  {
    return name.ToSnakeCase();
  }
}
