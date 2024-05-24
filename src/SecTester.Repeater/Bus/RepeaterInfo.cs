using MessagePack;

namespace SecTester.Repeater.Bus;

[MessagePackObject]
public sealed record RepeaterInfo
{
  [Key("repeaterId")]
  public string RepeaterId { get; set; } = null!;
}
