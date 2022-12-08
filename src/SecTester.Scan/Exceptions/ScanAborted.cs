using SecTester.Scan.Models;

namespace SecTester.Scan.Exceptions;

public class ScanAborted : ScanException
{
  public override ScanExceptionCode Type => ScanExceptionCode.Aborted;

  public ScanAborted(ScanStatus status) : base($"Scan {status.ToString().ToLowerInvariant()}.")
  {
  }
}
