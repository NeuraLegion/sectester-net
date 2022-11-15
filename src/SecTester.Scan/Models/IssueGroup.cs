namespace SecTester.Scan.Models;

public class IssueGroup
{
  public int Number { get; set; }
  public Severity Type { get; set; }

  public IssueGroup(int number, Severity type)
  {
    Number = number;
    Type = type;
  }
}
