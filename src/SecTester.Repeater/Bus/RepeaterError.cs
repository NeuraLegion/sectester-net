using MessagePack;

namespace SecTester.Repeater.Bus;

[MessagePackObject(true)]
public sealed record RepeaterError
{
  public string Message { get; set; } = null!;
}
