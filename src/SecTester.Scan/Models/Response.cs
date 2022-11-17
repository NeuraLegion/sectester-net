using System.Collections.Generic;

namespace SecTester.Scan.Models;

public record Response(Protocol? Protocol = default, int? Status = default,
  Dictionary<string, string>? Headers = default,
  string? Body = default)
{
}
