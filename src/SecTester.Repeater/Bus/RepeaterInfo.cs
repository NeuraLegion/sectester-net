using System;
using MessagePack;

namespace SecTester.Repeater.Bus;

[MessagePackObject]
public sealed record RepeaterInfo(string RepeaterId)
{
  [Key("repeaterId")]
  public string RepeaterId { get; init; } = RepeaterId ?? throw new ArgumentNullException(nameof(RepeaterId));
}
