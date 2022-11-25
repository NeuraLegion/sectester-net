using System;

namespace SecTester.Core.Utils;

public interface SystemTimeProvider
{
  DateTime Now { get; }
}
