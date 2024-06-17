using SecTester.Core;
using SecTester.Core.Commands;

namespace SecTester.Scan.Commands;

internal record DeleteScan : HttpRequest<Unit>
{
  public DeleteScan(string id)
    : base($"/api/v1/scans/{id}/delete", expectReply: false)
  {
  }
}
