using System.Timers;

namespace SecTester.Core.Utils;

public class SystemTimerProvider : Timer, TimerProvider
{
  public SystemTimerProvider()
  {
  }

  public SystemTimerProvider(double interval) : base(interval)
  {
  }
}
