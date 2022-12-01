using SecTester.Core.Bus;

namespace SecTester.Repeater.Bus;

[MessageType(name: "RepeaterStatusUpdated")]
public record RepeaterStatusEvent(string RepeaterId, RepeaterStatus Status) : Event;
