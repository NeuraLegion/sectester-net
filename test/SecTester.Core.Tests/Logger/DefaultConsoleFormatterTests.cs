using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SecTester.Core.Logger;
using SecTester.Core.Utils;

namespace SecTester.Core.Tests.Logger;

public class DefaultConsoleFormatterTests
{
  public static readonly object[][] HeaderLevelStrings =
  {
    new object[] { LogLevel.Trace, "TRACE      " }, 
    new object[] { LogLevel.Debug, "DEBUG      " },
    new object[] { LogLevel.Information, "INFORMATION" }, 
    new object[] { LogLevel.Warning, "WARNING    " },
    new object[] { LogLevel.Error, "ERROR      " }, 
    new object[] { LogLevel.Critical, "CRITICAL   " }
  };

  public static readonly object[][] HeaderDateTimeOutput =
  {
    new object[]
    {
      DateTime.Now, new DefaultConsoleFormatterOptions() { TimestampFormat = "HH:mm:ss", UseUtcTimestamp = true }
    },
    new object[]
    {
      DateTime.UtcNow, new DefaultConsoleFormatterOptions() { TimestampFormat = "HH:mm:ss", UseUtcTimestamp = true }
    },
    new object[]
    {
      DateTime.Now, new DefaultConsoleFormatterOptions() { TimestampFormat = "HH:mm:ss", UseUtcTimestamp = false }
    },
    new object[]
    {
      DateTime.UtcNow,
      new DefaultConsoleFormatterOptions() { TimestampFormat = "HH:mm:ss", UseUtcTimestamp = false }
    },
  };

  static LogEntry<Tuple<string, object[]>> CreateLogEntry(LogLevel logLevel, string message, params object[] args)
  {
    return new LogEntry<Tuple<string, object[]>>(logLevel, "category", 1, new Tuple<string, object[]>(message, args),
      null,
      (state, exception) => exception != null ? exception.ToString() : string.Format(state.Item1, state.Item2));
  }


  [Theory]
  [MemberData(nameof(HeaderDateTimeOutput))]
  public void Write_WithAnySystemTimeProvider_LogAdjustedDateTime(DateTime moment,
    DefaultConsoleFormatterOptions options)
  {
    // arrange
    var optionsMonitorMock = Substitute.For<IOptionsMonitor<DefaultConsoleFormatterOptions>>();
    var systemTimeProviderMock = Substitute.For<SystemTimeProvider>();
    var outStringWriter = new StringWriter();
    var logEntry = CreateLogEntry(LogLevel.Critical, "message");

    systemTimeProviderMock.Now.Returns(moment);
    optionsMonitorMock.CurrentValue.Returns(options);

    moment = moment.Kind == DateTimeKind.Local && options.UseUtcTimestamp ? moment.ToUniversalTime() : moment;
    moment = moment.Kind == DateTimeKind.Utc && !options.UseUtcTimestamp ? moment.ToLocalTime() : moment;

    var expectedHeader = $"[{moment.ToString(options.TimestampFormat, CultureInfo.InvariantCulture)}] ";

    var sut = new DefaultConsoleFormatter(optionsMonitorMock, systemTimeProviderMock);

    // act
    sut.Write(logEntry, null, outStringWriter);

    // assert
    outStringWriter.ToString().Should().StartWith(expectedHeader);
  }

  [Theory]
  [MemberData(nameof(HeaderLevelStrings))]
  public void Write_GivenLogLevel_LogLevelStringInHeader(LogLevel logLevel, string logLevelString)
  {
    // arrange
    var optionsMonitorMock = Substitute.For<IOptionsMonitor<DefaultConsoleFormatterOptions>>();
    var systemTimeProviderMock = Substitute.For<SystemTimeProvider>();
    var outStringWriter = new StringWriter();
    var logEntry = CreateLogEntry(logLevel, "message");

    systemTimeProviderMock.Now.Returns(DateTime.Now);
    optionsMonitorMock.CurrentValue.Returns(new DefaultConsoleFormatterOptions() { TimestampFormat = "  " });

    var expectedHeader = $"[  ] [{logLevelString}] message{Environment.NewLine}";

    var sut = new DefaultConsoleFormatter(optionsMonitorMock, systemTimeProviderMock);

    // act
    sut.Write(logEntry, null, outStringWriter);

    // assert
    outStringWriter.ToString().Should().Be(expectedHeader);
  }
}
