using System;
using System.Collections.Generic;

namespace SecTester.Scan.Models;

public record Request(string Url, RequestMethod? Method = default, Dictionary<string, string>? Headers = default,
  string? Body = default, Protocol? Protocol = default)
{
  public string Url { get; init; } = Url ?? throw new ArgumentNullException(nameof(Url));
}
