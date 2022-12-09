using SecTester.Scan.Models;

namespace SecTester.Reporter;

public interface Formatter
{
  string Format(Issue issue);
}

