using System.Collections.Generic;

namespace SecTester.Scan.Models;

public class Response
{
  public Protocol? Protocol { get; set; }
  public int? Status { get; set; }
  public Dictionary<string, string>? Headers { get; set; }
  public string? Body { get; set; }

  public Response(Protocol? protocol = default, int? status = default, Dictionary<string, string>? headers = default,
    string? body = default)
  {
    Protocol = protocol;
    Status = status;
    Headers = headers;
    Body = body;
  }
}
