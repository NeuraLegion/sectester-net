namespace SecTester.Repeater.Bus;

public record RegisterRepeaterPayload(string? Version = default, RepeaterRegisteringError Error = RepeaterRegisteringError.None);
