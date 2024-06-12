using SecTester.Repeater.Commands;
using SecTester.Scan.Models;

namespace SecTester.Scan.Commands;

internal record GetScan : HttpRequest<ScanState>
{
  public GetScan(string id)
    : base($"/api/v1/scans/{id}")
  {
  }
}
