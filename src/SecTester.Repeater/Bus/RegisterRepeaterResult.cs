namespace SecTester.Repeater.Bus;

public record RegisterRepeaterResult(string? Version, RepeaterRegisteringError Error = RepeaterRegisteringError.None);
