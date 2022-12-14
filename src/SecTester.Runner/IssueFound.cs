using SecTester.Core.Exceptions;
using SecTester.Reporter;
using SecTester.Scan.Models;

namespace SecTester.Runner;

public class IssueFound : SecTesterException
{
  public Issue Issue { get; }

  public IssueFound(Issue issue, IFormatter formatter) :
    base($"Target is vulnerable\n\n{formatter.Format(issue)}")
  {
    this.Issue = issue;
  }
}
