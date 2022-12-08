namespace SecTester.Scan.Models.HarSpec;

public record QueryParameter(string Name, string Value) : Parameter(Name, Value);
