namespace SecTester.Scan.Target.HarSpec;

public record QueryParameter(string Name, string Value) : Parameter(Name, Value);
