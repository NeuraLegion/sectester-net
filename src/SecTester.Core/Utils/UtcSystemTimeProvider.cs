using System;

namespace SecTester.Core.Utils;

public class UtcSystemTimeProvider : ISystemTimeProvider
{
  public DateTime Now => DateTime.UtcNow;
}
