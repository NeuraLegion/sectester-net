namespace SecTester.Repeater.Bus;

public record RegisterRepeaterResult(string? Version = default, RepeaterRegisteringError Error = RepeaterRegisteringError.None);
