using SecTester.Core.Utils;

namespace SecTester.Repeater.Internal;

internal class MessagePackSnakeCaseNamingPolicy : MessagePackNamingPolicy
{
  public override string ConvertName(string name)
  {
    return name.ToSnakeCase();
  }
}
