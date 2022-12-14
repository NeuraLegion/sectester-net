using System.Timers;

namespace SecTester.Core.Utils;

public interface ITimerProvider
{
  void Start();
  void Stop();
  double Interval { get; set; }
  event ElapsedEventHandler Elapsed;
}
