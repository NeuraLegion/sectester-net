namespace SecTester.Core.Tests.Logger;

public class DefaultConsoleFormatterTests
{
  public static readonly object[][] HeaderLevelStrings =
  {
    new object[] { LogLevel.Trace, "TRACE      " }, new object[] { LogLevel.Debug, "DEBUG      " },
    new object[] { LogLevel.Information, "INFORMATION" }, new object[] { LogLevel.Warning, "WARNING    " },
    new object[] { LogLevel.Error, "ERROR      " }, new object[] { LogLevel.Critical, "CRITICAL   " }
  };

  public static readonly object[][] HeaderDateTimeOutput =
  {
    new object[]
    {
      DateTime.Now,
      new ConsoleFormatterOptions() { TimestampFormat = "HH:mm:ss", UseUtcTimestamp = true, IncludeScopes = false }
    },
    new object[]
    {
      DateTime.UtcNow,
      new ConsoleFormatterOptions() { TimestampFormat = "HH:mm:ss", UseUtcTimestamp = true, IncludeScopes = false }
    },
    new object[]
    {
      DateTime.Now,
      new ConsoleFormatterOptions() { TimestampFormat = "HH:mm:ss", UseUtcTimestamp = false, IncludeScopes = false }
    },
    new object[]
    {
      DateTime.UtcNow,
      new ConsoleFormatterOptions() { TimestampFormat = "HH:mm:ss", UseUtcTimestamp = false, IncludeScopes = false }
    },
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
  [MemberData(nameof(HeaderDateTimeOutput))]
  public void Write_WithAnySystemTimeProvider_LogAdjustedDateTime(DateTime moment,
    ConsoleFormatterOptions options)
  {
    // arrange
    var optionsMonitorMock = Substitute.For<IOptionsMonitor<ConsoleFormatterOptions>>();
    var systemTimeProviderMock = Substitute.For<SystemTimeProvider>();
    var outStringWriter = new StringWriter();
    var logEntry = CreateLogEntry(LogLevel.Critical, "message");

    systemTimeProviderMock.Now.Returns(moment);
    optionsMonitorMock.CurrentValue.Returns(options);

    moment = moment.Kind == DateTimeKind.Local && options.UseUtcTimestamp ? moment.ToUniversalTime() : moment;
    moment = moment.Kind == DateTimeKind.Utc && !options.UseUtcTimestamp ? moment.ToLocalTime() : moment;

    using var sut = new DefaultConsoleFormatter(optionsMonitorMock, systemTimeProviderMock);

    // act
    sut.Write(logEntry, _externalScopeProviderMock, outStringWriter);

    // assert
    outStringWriter.ToString().Should()
      .StartWith($"[{moment.ToString(options.TimestampFormat, CultureInfo.InvariantCulture)}] ");
  }

  [Theory]
  [MemberData(nameof(HeaderLevelStrings))]
  public void Write_GivenLogLevel_LogLevelStringInHeader(LogLevel logLevel, string logLevelString)
  {
    // arrange
    var optionsMonitorMock = Substitute.For<IOptionsMonitor<ConsoleFormatterOptions>>();
    var systemTimeProviderMock = Substitute.For<SystemTimeProvider>();
    var outStringWriter = new StringWriter();
    var logEntry = CreateLogEntry(logLevel, "message");

    systemTimeProviderMock.Now.Returns(DateTime.Now);
    optionsMonitorMock.CurrentValue.Returns(new ConsoleFormatterOptions()
    {
      TimestampFormat = "  ",
      IncludeScopes = false,
      UseUtcTimestamp = false
    });

    using var sut = new DefaultConsoleFormatter(optionsMonitorMock, systemTimeProviderMock);

    // act
    sut.Write(logEntry, _externalScopeProviderMock, outStringWriter);

    // assert
    outStringWriter.ToString().Should().Be($"[  ] [{logLevelString}] message{Environment.NewLine}");
  }

  [Fact]
  public void Write_GivenEmptyMessage_SkipLog()
  {
    // arrange
    var optionsMonitorMock = Substitute.For<IOptionsMonitor<ConsoleFormatterOptions>>();
    var systemTimeProviderMock = Substitute.For<SystemTimeProvider>();
    var outStringWriter = new StringWriter();
    var logEntry = CreateLogEntry(LogLevel.Critical, "");

    systemTimeProviderMock.Now.Returns(DateTime.Now);
    optionsMonitorMock.CurrentValue.Returns(new ConsoleFormatterOptions()
    {
      TimestampFormat = "  ",
      IncludeScopes = false,
      UseUtcTimestamp = false
    });

    using var sut = new DefaultConsoleFormatter(optionsMonitorMock, systemTimeProviderMock);

    // act
    sut.Write(logEntry, _externalScopeProviderMock, outStringWriter);

    // assert
    outStringWriter.ToString().Should().BeEmpty();
  }
}
