using System.Collections.Generic;
using SecTester.Bus.Commands;
using SecTester.Scan.Models;

namespace SecTester.Scan.Commands;

internal record ListIssues : HttpRequest<IEnumerable<Issue>>
{
  public ListIssues(string id)
    : base($"/api/v1/scans/{id}/issues")
  {
  }
}
