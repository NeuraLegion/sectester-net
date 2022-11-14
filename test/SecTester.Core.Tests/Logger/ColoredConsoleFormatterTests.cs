using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using SecTester.Core.Logger;
using SecTester.Core.Utils;

namespace SecTester.Core.Tests.Logger;

public class ColoredConsoleFormatterTests
{
  const string DefaultForegroundColor = "\x1B[39m\x1B[22m";

  public static readonly object[][] HeaderColors =
  {
    new object[] { LogLevel.Critical, "\x1B[1m\x1B[31m" }, new object[] { LogLevel.Error, "\x1B[31m" },
    new object[] { LogLevel.Warning, "\x1B[1m\x1B[33m" }, new object[] { LogLevel.Information, "\x1B[32m" },
    new object[] { LogLevel.Debug, "\x1B[1m\x1B[37m" }, new object[] { LogLevel.Trace, "\x1B[1m\x1B[36m" }
  };

  private readonly IExternalScopeProvider _externalScopeProviderMock = Substitute.For<IExternalScopeProvider>();

  private static LogEntry<Tuple<string, object[]>> CreateLogEntry(LogLevel logLevel, string message,
    params object[] args)
  {
    return new LogEntry<Tuple<string, object[]>>(logLevel, "category", 1, new Tuple<string, object[]>(message, args),
      null,
      (state, exception) => exception?.ToString() ?? string.Format(state.Item1, state.Item2));
  }

  [Theory]
  [MemberData(nameof(HeaderColors))]
  public void Write_GivenLogLevel_LogColorSequence(LogLevel logLevel, string foregroundAnsiColor)
  {
    // arrange
    var optionsMonitorMock = Substitute.For<IOptionsMonitor<ConsoleFormatterOptions>>();
    var systemTimeProviderMock = Substitute.For<SystemTimeProvider>();
    var outStringWriter = new StringWriter();
    var logEntry = CreateLogEntry(logLevel, "message");

    systemTimeProviderMock.Now.Returns(DateTime.Now);
    optionsMonitorMock.CurrentValue.Returns(new ConsoleFormatterOptions()
    {
      TimestampFormat = "HH:mm:ss", UseUtcTimestamp = false, IncludeScopes = false
    });

    using var sut = new ColoredConsoleFormatter(optionsMonitorMock, systemTimeProviderMock);

    // act
    sut.Write(logEntry, _externalScopeProviderMock, outStringWriter);

    // assert
    outStringWriter.ToString().Should().StartWith(foregroundAnsiColor);
    outStringWriter.ToString().Should().EndWith($"{DefaultForegroundColor} message{Environment.NewLine}");
  }
}
