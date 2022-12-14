using SecTester.Scan.Models;

namespace SecTester.Reporter;

public interface IFormatter
{
  string Format(Issue issue);
}

