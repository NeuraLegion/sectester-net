using SecTester.Bus.Commands;
using SecTester.Core;

namespace SecTester.Scan.Commands;

internal record StopScan : HttpRequest<Unit>
{
  public StopScan(string id)
    : base($"/api/v1/scans/{id}/stop", expectReply: false)
  {
  }
}
