using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using SecTester.Core.Utils;

namespace SecTester.Core.Logger;

public class ColoredConsoleFormatter : DefaultConsoleFormatter
{
  private readonly AnsiCodeColorizer _ansiCodeColorizer;

  public ColoredConsoleFormatter(IOptionsMonitor<ConsoleFormatterOptions> options, SystemTimeProvider systemTimeProvider,
    AnsiCodeColorizer ansiCodeColorizer)
    : base(nameof(ColoredConsoleFormatter), options, systemTimeProvider)
  {
    _ansiCodeColorizer = ansiCodeColorizer;
  }

  protected override void WriteHeader<TState>(
    in LogEntry<TState> logEntry,
    TextWriter textWriter)
  {
    textWriter.Write(
      _ansiCodeColorizer.Colorize(
        GetForegroundColor(logEntry.LogLevel),
        FormatHeader(logEntry.LogLevel)));
  }

  static AnsiCodeColor GetForegroundColor(LogLevel level) =>
    level switch
    {
      LogLevel.Critical => AnsiCodeColor.Red,
      LogLevel.Error => AnsiCodeColor.DarkRed,
      LogLevel.Warning => AnsiCodeColor.Yellow,
      LogLevel.Information => AnsiCodeColor.DarkGreen,
      LogLevel.Debug => AnsiCodeColor.White,
      LogLevel.Trace => AnsiCodeColor.Cyan,

      _ => AnsiCodeColor.DefaultForeground
    };
}
