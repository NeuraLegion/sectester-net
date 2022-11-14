using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using SecTester.Core.Utils;

namespace SecTester.Core.Logger;

public class ColoredConsoleFormatter : DefaultConsoleFormatter
{
  const string DefaultForegroundColor = "\x1B[39m\x1B[22m";


  public ColoredConsoleFormatter(IOptionsMonitor<ConsoleFormatterOptions> options, SystemTimeProvider systemTimeProvider)
    : base(nameof(ColoredConsoleFormatter), options, systemTimeProvider)
  {
  }

  protected override void WriteHeader<TState>(
    in LogEntry<TState> logEntry,
    TextWriter textWriter)
  {
      textWriter.Write(GetForegroundColorAnsiCode(logEntry.LogLevel));
      textWriter.Write(FormatHeader(logEntry.LogLevel));
      textWriter.Write(DefaultForegroundColor);  
  }

  static string GetForegroundColorAnsiCode(LogLevel level) =>
    level switch
    {
      LogLevel.Critical  => "\x1B[1m\x1B[31m",  // ConsoleColor.Red
      LogLevel.Error => "\x1B[31m",             // ConsoleColor.DarkRed
      LogLevel.Warning => "\x1B[1m\x1B[33m",    // ConsoleColor.Yellow
      LogLevel.Information => "\x1B[32m",       // ConsoleColor.DarkGreen
      LogLevel.Debug =>  "\x1B[1m\x1B[37m",     // ConsoleColor.White
      LogLevel.Trace =>  "\x1B[1m\x1B[36m",     // ConsoleColor.Cyan
      
      _ => DefaultForegroundColor
    };
}
