using System;

namespace SecTester.Core.Utils;

public interface ISystemTimeProvider
{
  DateTime Now { get; }
}
