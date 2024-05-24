using MessagePack;

namespace SecTester.Repeater.Bus;

[MessagePackObject(true)]
public sealed record RepeaterVersion
{
  [Key("version")]
  public string Version { get; set; } = null!;
}
