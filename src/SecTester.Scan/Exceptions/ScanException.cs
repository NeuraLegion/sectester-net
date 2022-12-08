using SecTester.Core.Exceptions;

namespace SecTester.Scan.Exceptions;

public abstract class ScanException : SecTesterException
{
  public abstract ScanExceptionCode Type { get; }

  protected ScanException(string message) : base(message)
  {
  }
}
