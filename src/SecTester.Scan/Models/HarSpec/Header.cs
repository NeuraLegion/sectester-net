namespace SecTester.Scan.Models.HarSpec;

public record Header(string Name, string Value) : Parameter(Name, Value);
