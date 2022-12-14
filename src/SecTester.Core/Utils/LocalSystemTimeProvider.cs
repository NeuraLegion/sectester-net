using System;

namespace SecTester.Core.Utils;

public class LocalSystemTimeProvider : ISystemTimeProvider
{
  public DateTime Now => DateTime.Now;
}
