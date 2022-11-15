using System;
using System.Collections.Generic;

namespace SecTester.Scan.Models;

public class Request
{
  public string Url { get; set; }
  public RequestMethod? Method { get; set; }
  public Dictionary<string, string>? Headers { get; set; }
  public string? Body { get; set; }
  public Protocol? Protocol { get; set; }

  public Request(string url, RequestMethod? method = default, Dictionary<string, string>? headers = default,
    string? body = default, Protocol? protocol = default)
  {
    Url = url ?? throw new ArgumentNullException(nameof(url));
    Method = method;
    Headers = headers;
    Body = body;
    Protocol = protocol;
  }
}
