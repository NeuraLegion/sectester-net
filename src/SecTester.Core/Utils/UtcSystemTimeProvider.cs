using System;

namespace SecTester.Core.Utils;

public class UtcSystemTimeProvider : SystemTimeProvider
{
  public DateTime Now => DateTime.UtcNow;
}
