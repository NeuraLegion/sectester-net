using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SecTester.Core.Utils;

namespace SecTester.Core.Logger;

public class DefaultLogger : ILogger
{
  private const string DatetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

  private static readonly Dictionary<LogLevel, string> Levels = Enum
    .GetValues(typeof(LogLevel))
    .Cast<LogLevel>()
    .ToDictionary(x => x, x => Enum.GetName(typeof(LogLevel), x));

  private static readonly int MaxFormattedLevelLength = Levels
    .Select(x => x.Value)
    .OrderByDescending(x => x.Length)
    .First()
    .Length;

  private static readonly Dictionary<LogLevel, string> FormattedLevels = Levels.ToDictionary(
    x => x.Key, x => x.Value.ToUpper(CultureInfo.InvariantCulture).PadRight(MaxFormattedLevelLength));


  private readonly ISystemTimeProvider _systemTimeProvider;
  private readonly ILogAppender _logAppender;
  private volatile LogLevel _logLevel;

  public LogLevel LogLevel
  {
    get => _logLevel;
    set => _logLevel = value;
  }

  public DefaultLogger(ISystemTimeProvider systemTimeProvider, ILogAppender logAppender,
    LogLevel logLevel = LogLevel.Notice)
  {
    _systemTimeProvider = systemTimeProvider;
    _logAppender = logAppender;
    _logLevel = logLevel;
  }


  public void Error(string message, params object[] args)
  {
    Write(LogLevel.Error, message, args);
  }

  public void Warn(string message, params object[] args)
  {
    Write(LogLevel.Warn, message, args);
  }

  public void Log(string message, params object[] args)
  {
    Write(LogLevel.Notice, message, args);
  }

  public void Debug(string message, params object[] args)
  {
    Write(LogLevel.Verbose, message, args);
  }

  private void Write(LogLevel level, string message, params object[] args)
  {
    if (LogLevel < level)
    {
      return;
    }

    _logAppender.WriteLine(level,
      $"{FormatHeader(level)} - {string.Format(CultureInfo.InvariantCulture, message, args)}");
  }

  private string FormatHeader(LogLevel level)
  {
    return
      $"[{_systemTimeProvider.Now.ToString(DatetimeFormat, CultureInfo.InvariantCulture)}] [{FormattedLevels[level]}]";
  }
}
