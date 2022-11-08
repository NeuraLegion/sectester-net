using System;
using System.Threading;

namespace SecTester.Core.Logger;

public class ColoredConsoleLogAppender : ILogAppender
{
  private static SpinLock _consoleLock = new(false);

  public void WriteLine(LogLevel logLevel, string message)
  {
    var color = GetColor(logLevel);
    var lockTaken = false;

    try
    {
      var stream = LogLevel.Error == logLevel ? Console.Error : Console.Out;

      _consoleLock.Enter(ref lockTaken);

      Console.ForegroundColor = color;

      stream.WriteLine(message);

      Console.ResetColor();
    }
    finally
    {
      if (lockTaken)
      {
        _consoleLock.Exit();
      }
    }
  }

  private static ConsoleColor GetColor(LogLevel level)
  {
    switch (level)
    {
      case LogLevel.Error:
        return ConsoleColor.Red;
      case LogLevel.Warn:
        return ConsoleColor.Yellow;
      case LogLevel.Notice:
        return ConsoleColor.Green;
      case LogLevel.Verbose:
        return ConsoleColor.Cyan;
      default:
        return ConsoleColor.Gray;
    }
  }
}
