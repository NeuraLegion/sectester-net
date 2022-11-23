using SecTester.Bus.Commands;
using SecTester.Core;

namespace SecTester.Scan.Commands;

public record DeleteScan : HttpRequest<Unit>
{
  public DeleteScan(string id, int? ttl = default)
    : base($"/api/v1/scans/{id}/delete", ttl: ttl)
  {
  }
}
