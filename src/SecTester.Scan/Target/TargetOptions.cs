using System;
using System.Collections.Generic;
using System.Net.Http;
using SecTester.Scan.Models;

namespace SecTester.Scan.Target;

public record TargetOptions(string Url, Dictionary<string, IEnumerable<string>>? Query, HttpContent? Body, RequestMethod? Method,  Dictionary<string, IEnumerable<string>>? Headers)
{
  private string Url { get; init; } = Url ?? throw new ArgumentNullException(nameof(Url)); 
}
