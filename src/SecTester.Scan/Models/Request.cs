using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SecTester.Scan.Models;

public record Request(string Url)
{
  public string Url { get; init; } = Url ?? throw new ArgumentNullException(nameof(Url));
  public HttpMethod? Method { get; init; }
  public IDictionary<string, string>? Headers { get; init; }
  public string? Body { get; init; }
  public Protocol? Protocol { get; init; }
}
