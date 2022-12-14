using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using SecTester.Core.Utils;

namespace SecTester.Core.Logger;

public class DefaultConsoleFormatter : ConsoleFormatter, IDisposable
{
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


  private readonly IDisposable _optionsReloadToken;
  private ConsoleFormatterOptions _formatterOptions;

  private readonly ISystemTimeProvider _systemTimeProvider;

  public DefaultConsoleFormatter(IOptionsMonitor<ConsoleFormatterOptions> options,
    ISystemTimeProvider systemTimeProvider)
    : this(nameof(DefaultConsoleFormatter), options, systemTimeProvider)
  {
  }

  public DefaultConsoleFormatter(string name, IOptionsMonitor<ConsoleFormatterOptions> options, ISystemTimeProvider systemTimeProvider)
    : base(name)
  {
    _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    _formatterOptions = options.CurrentValue;
    _systemTimeProvider = systemTimeProvider;
  }

  [ExcludeFromCodeCoverage]
  private void ReloadLoggerOptions(ConsoleFormatterOptions options)
  {
    _formatterOptions = options;
  }

  public override void Write<TState>(
    in LogEntry<TState> logEntry,
    IExternalScopeProvider scopeProvider,
    TextWriter textWriter)
  {
    string? message =
      logEntry.Formatter?.Invoke(
        logEntry.State, logEntry.Exception);

    if (message is null || message.Length == 0)
    {
      return;
    }

    WriteHeader(logEntry, textWriter);
    textWriter.Write(" ");
    textWriter.WriteLine(message);
  }

  protected virtual void WriteHeader<TState>(
    in LogEntry<TState> logEntry,
    TextWriter textWriter)
  {
    textWriter.Write(FormatHeader(logEntry.LogLevel));
  }

  private DateTime GetTimestamp()
  {
    DateTime timestamp = _systemTimeProvider.Now;

    if (_formatterOptions.UseUtcTimestamp)
    {
      timestamp = timestamp.Kind == DateTimeKind.Utc ? timestamp : timestamp.ToUniversalTime();
    }
    else
    {
      timestamp = timestamp.Kind != DateTimeKind.Utc ? timestamp : timestamp.ToLocalTime();
    }

    return timestamp;
  }

  protected string FormatHeader(LogLevel level)
  {
    return
      $"[{GetTimestamp().ToString(_formatterOptions.TimestampFormat, CultureInfo.InvariantCulture)}] [{FormattedLevels[level]}]";
  }

  public void Dispose()
  {
    _optionsReloadToken.Dispose();
    GC.SuppressFinalize(this);
  }
}
