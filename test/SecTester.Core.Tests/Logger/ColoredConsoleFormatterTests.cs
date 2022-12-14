namespace SecTester.Core.Tests.Logger;

public class ColoredConsoleFormatterTests : IDisposable
{
  const string DefaultForegroundColor = "\x1B[39m\x1B[22m";

  public static readonly object[][] HeaderColors =
  {
    new object[] { LogLevel.Critical, "\x1B[1m\x1B[31m" }, new object[] { LogLevel.Error, "\x1B[31m" },
    new object[] { LogLevel.Warning, "\x1B[1m\x1B[33m" }, new object[] { LogLevel.Information, "\x1B[32m" },
    new object[] { LogLevel.Debug, "\x1B[1m\x1B[37m" }, new object[] { LogLevel.Trace, "\x1B[1m\x1B[36m" }
  };

  private readonly IExternalScopeProvider _externalScopeProviderMock = Substitute.For<IExternalScopeProvider>();
  private readonly IAnsiCodeColorizer _ansiCodeColorizer = Substitute.For<IAnsiCodeColorizer>();

  private static LogEntry<Tuple<string, object[]>> CreateLogEntry(LogLevel logLevel, string message,
    params object[] args)
  {
    return new LogEntry<Tuple<string, object[]>>(logLevel, "category", 1, new Tuple<string, object[]>(message, args),
      null,
      (state, exception) => exception?.ToString() ?? string.Format(state.Item1, state.Item2));
  }

  public void Dispose()
  {
    _externalScopeProviderMock.ClearSubstitute();
    _ansiCodeColorizer.ClearSubstitute();

    GC.SuppressFinalize(this);
  }

  [Theory]
  [MemberData(nameof(HeaderColors))]
  public void Write_GivenLogLevel_LogsColorSequence(LogLevel logLevel, string foregroundAnsiColor)
  {
    // arrange
    var optionsMonitorMock = Substitute.For<IOptionsMonitor<ConsoleFormatterOptions>>();
    var systemTimeProviderMock = Substitute.For<ISystemTimeProvider>();
    var outStringWriter = new StringWriter();
    var logEntry = CreateLogEntry(logLevel, "message");

    systemTimeProviderMock.Now.Returns(DateTime.Now);
    optionsMonitorMock.CurrentValue.Returns(new ConsoleFormatterOptions
    {
      TimestampFormat = "HH:mm:ss",
      UseUtcTimestamp = false,
      IncludeScopes = false
    });

    _ansiCodeColorizer.Colorize(Arg.Any<AnsiCodeColor>(), Arg.Any<string>())
      .Returns(x => $"{x.ArgAt<AnsiCodeColor>(0)}{x.ArgAt<string>(1)}{AnsiCodeColor.DefaultForeground}");

    using var sut = new ColoredConsoleFormatter(optionsMonitorMock, systemTimeProviderMock, _ansiCodeColorizer);

    // act
    sut.Write(logEntry, _externalScopeProviderMock, outStringWriter);

    // assert
    var result = outStringWriter.ToString();
    result.Should().StartWith(foregroundAnsiColor);
    result.Should().EndWith($"{DefaultForegroundColor} message{Environment.NewLine}");
  }

  [Theory]
  [MemberData(nameof(HeaderColors))]
  public void Write_GivenLogLevel_DoesNotLogColorSequence(LogLevel logLevel, string foregroundAnsiColor)
  {
    // arrange
    var optionsMonitorMock = Substitute.For<IOptionsMonitor<ConsoleFormatterOptions>>();
    var systemTimeProviderMock = Substitute.For<ISystemTimeProvider>();
    var outStringWriter = new StringWriter();
    var logEntry = CreateLogEntry(logLevel, "message");

    systemTimeProviderMock.Now.Returns(DateTime.Now);
    optionsMonitorMock.CurrentValue.Returns(new ConsoleFormatterOptions
    {
      TimestampFormat = "HH:mm:ss",
      UseUtcTimestamp = false,
      IncludeScopes = false
    });

    _ansiCodeColorizer.Colorize(Arg.Any<AnsiCodeColor>(), Arg.Any<string>())
      .Returns(x => x.ArgAt<string>(1));

    using var sut = new ColoredConsoleFormatter(optionsMonitorMock, systemTimeProviderMock, _ansiCodeColorizer);

    // act
    sut.Write(logEntry, _externalScopeProviderMock, outStringWriter);

    // assert
    var result = outStringWriter.ToString();
    result.Should().NotContain(foregroundAnsiColor);
    result.Should().NotContain(DefaultForegroundColor);
    result.Should().EndWith($" message{Environment.NewLine}");
  }
}
