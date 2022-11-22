using System.Collections.Generic;

namespace SecTester.Scan.Target.Har;

public record Message
{
  public int HeadersSize { get; init; }
  public int? BodySize { get; init; }
  public IEnumerable<Cookie> Cookies { get; init; } = new List<Cookie>();
  public IEnumerable<Header> Headers { get; init; } = new List<Header>();
  public string HttpVersion { get; init; }
}
