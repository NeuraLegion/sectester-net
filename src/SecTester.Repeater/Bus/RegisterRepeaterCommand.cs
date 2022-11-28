using SecTester.Core.Bus;

namespace SecTester.Repeater.Bus;

[MessageType(name: "RepeaterRegistering")]
public record RegisterRepeaterCommand(string Version, string RepeaterId) : Command<RegisterRepeaterResult>;
