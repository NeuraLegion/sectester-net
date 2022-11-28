using System.Collections.Generic;

namespace SecTester.Scan.Target.Har;

public record Message
{
  public int HeadersSize { get; init; } = -1;
  public int? BodySize { get; init; } = -1;
  public IEnumerable<Cookie> Cookies { get; init; } = new List<Cookie>();
  public IEnumerable<Header> Headers { get; init; } = new List<Header>();
  public string HttpVersion { get; init; } = "HTTP/0.9";
}
