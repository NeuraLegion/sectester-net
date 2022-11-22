namespace SecTester.Scan.Target.Har;

public record Header(string Name, string Value) : Parameter(Name, Value);
