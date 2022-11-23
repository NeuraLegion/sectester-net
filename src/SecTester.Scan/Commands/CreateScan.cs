using System.Net.Http;
using SecTester.Bus.Commands;
using SecTester.Scan.Models;

namespace SecTester.Scan.Commands;

public record CreateScan : HttpRequest<Identifiable<string>>
{
  public CreateScan(HttpContent body, int? ttl = null)
    : base("/api/v1/scans", HttpMethod.Post, body: body, expectReply: true, ttl: ttl)
  {
  }
}
