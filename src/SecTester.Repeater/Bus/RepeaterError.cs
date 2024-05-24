using MessagePack;

namespace SecTester.Repeater.Bus;

[MessagePackObject]
public sealed record RepeaterError
{
  [Key("message")]
  public string Message { get; set; } = null!;
}
