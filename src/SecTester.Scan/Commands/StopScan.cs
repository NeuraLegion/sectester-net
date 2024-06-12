using SecTester.Core;
using SecTester.Repeater.Commands;

namespace SecTester.Scan.Commands;

internal record StopScan : HttpRequest<Unit>
{
  public StopScan(string id)
    : base($"/api/v1/scans/{id}/stop", expectReply: false)
  {
  }
}
