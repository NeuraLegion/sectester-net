using System.Collections.Generic;
using SecTester.Bus.Commands;
using SecTester.Scan.Models;

namespace SecTester.Scan.Commands;

public record ListIssues : HttpRequest<IEnumerable<Issue>>
{
  public ListIssues(string id, int? ttl = default)
    : base($"/api/v1/scans/{id}/issues", expectReply: true, ttl: ttl)
  {
  }
}
