using System;

namespace SecTester.Scan.Exceptions;

public class ScanTimedOut : ScanException
{
  public override ScanExceptionCode Type => ScanExceptionCode.TimedOut;

  public ScanTimedOut(TimeSpan timeout, Exception? innerException) : base($"The expectation was not satisfied within the {timeout.TotalMilliseconds} ms timeout specified.", innerException)
  {
  }
}
