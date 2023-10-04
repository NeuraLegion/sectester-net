using MessagePack;

namespace SecTester.Repeater.Bus;

[MessagePackObject(true)]
public sealed record RepeaterVersion
{
  public string Version { get; set; } = null!;
}
