namespace SecTester.Scan.Target.Har;

public record QueryParameter(string Name, string Value) : Parameter(Name, Value);
