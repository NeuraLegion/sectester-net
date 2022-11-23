using SecTester.Bus.Commands;
using SecTester.Scan.Models;

namespace SecTester.Scan.Commands;

public record GetScan : HttpRequest<ScanState>
{
  public GetScan(string id, int? ttl = default)
    : base($"/api/v1/scans/{id}", expectReply: true, ttl: ttl)
  {
  }
}
