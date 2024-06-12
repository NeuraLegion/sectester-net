using System.Text.Json;
using SecTester.Core.Utils;

namespace SecTester.Repeater.Internal;

internal class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
{
  public static JsonNamingPolicy Instance { get; } = new JsonSnakeCaseNamingPolicy();

  public override string ConvertName(string name)
  {
    return name.ToSnakeCase();
  }
}
