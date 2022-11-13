using System;

namespace SecTester.Core.Utils;

public class LocalSystemTimeProvider : SystemTimeProvider
{
  public DateTime Now => DateTime.Now;
}
