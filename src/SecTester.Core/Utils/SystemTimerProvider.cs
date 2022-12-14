using System.Timers;

namespace SecTester.Core.Utils;

public class SystemTimerProvider : Timer, ITimerProvider
{
  public SystemTimerProvider()
  {
  }

  public SystemTimerProvider(double interval) : base(interval)
  {
  }
}
