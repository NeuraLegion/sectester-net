namespace SecTester.Repeater.Internal;

internal abstract class MessagePackNamingPolicy
{
  public static MessagePackNamingPolicy SnakeCase { get; } = new MessagePackSnakeCaseNamingPolicy();

  public abstract string ConvertName(string name);
}
