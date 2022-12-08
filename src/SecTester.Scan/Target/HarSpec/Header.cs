namespace SecTester.Scan.Target.HarSpec;

public record Header(string Name, string Value) : Parameter(Name, Value);
