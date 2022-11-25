using System.Collections.Generic;

namespace SecTester.Scan.Models;

public record Response(Protocol? Protocol = default, int? Status = default,
  IDictionary<string, string>? Headers = default,
  string? Body = default);
