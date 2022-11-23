using SecTester.Bus.Commands;
using SecTester.Core;

namespace SecTester.Scan.Commands;

public record StopScan : HttpRequest<Unit>
{
  public StopScan(string id, int? ttl = default)
    : base($"/api/v1/scans/{id}/stop", ttl: ttl)
  {
  }
}
