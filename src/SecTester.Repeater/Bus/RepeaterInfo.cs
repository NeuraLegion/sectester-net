using MessagePack;

namespace SecTester.Repeater.Bus;

[MessagePackObject(true)]
public sealed record RepeaterInfo
{
  public string RepeaterId { get; set; } = null!;
}
