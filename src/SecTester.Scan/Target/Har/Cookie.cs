using System;

namespace SecTester.Scan.Target.Har;

public record Cookie(string Name, string Value) : Parameter(Name, Value)
{
  public string? Path { get; init; }
  public string? Domain { get; init; }
  public bool? HttpOnly { get; init; }
  public bool? Secure { get; init; }
  public DateTime? Expires { get; init; }
}
