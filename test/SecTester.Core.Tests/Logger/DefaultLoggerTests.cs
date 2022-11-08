using System.Globalization;
using SecTester.Core.Logger;
using SecTester.Core.Utils;
using SecTester.Core.Extensions;

namespace SecTester.Core.Tests.Logger;

public class DefaultLoggerTests : IDisposable
{
  private const string Message = "message";
  private const string DatetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

  private static readonly DateTime Moment;
  private static readonly string HeaderLogLevelError;
  private static readonly string HeaderLogLevelNotice;
  private static readonly string HeaderLogLevelWarn;
  private static readonly string HeaderLogLevelVerbose;

  private readonly ILogAppender _logAppenderMock;
  private ServiceProvider _serviceProvider;

  static DefaultLoggerTests()
  {
    Moment = DateTime.Now;
    var moment = Moment.ToString(DatetimeFormat, CultureInfo.InvariantCulture);

    HeaderLogLevelError = $"[{moment}] [ERROR  ] - ";
    HeaderLogLevelNotice = $"[{moment}] [NOTICE ] - ";
    HeaderLogLevelVerbose = $"[{moment}] [VERBOSE] - ";
    HeaderLogLevelWarn = $"[{moment}] [WARN   ] - ";
  }

  public DefaultLoggerTests()
  {
    var systemTimeProviderMock = Substitute.For<ISystemTimeProvider>();
    systemTimeProviderMock.Now.Returns(Moment);
    _logAppenderMock = Substitute.For<ILogAppender>();
    _serviceProvider = new ServiceCollection()
      .AddSingleton(_logAppenderMock)
      .AddSingleton(systemTimeProviderMock)
      .AddTransient<DefaultLogger>()
      .BuildServiceProvider();
  }

  public void Dispose()
  {
    _serviceProvider.Dispose();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public void Constructor_SetALevelToNoticeByDefault()
  {
    // act
    var sut = _serviceProvider.ResolveWith<DefaultLogger>();

    // assert
    sut.LogLevel.Should().Be(LogLevel.Notice);
  }

  [Fact]
  public void Constructor_SetACustomLevel()
  {
    // act
    var sut = _serviceProvider.ResolveWith<DefaultLogger>(LogLevel.Error);

    // assert
    sut.LogLevel.Should().Be(LogLevel.Error);
  }

  [Fact]
  public void LogLevel_ChangeADefaultLevel()
  {
    // arrange
    var sut = _serviceProvider.ResolveWith<DefaultLogger>();

    // act
    sut.LogLevel = LogLevel.Warn;

    // assert
    sut.LogLevel.Should().Be(LogLevel.Warn);
  }

  [Theory]
  [InlineData(LogLevel.Silent)]
  [InlineData(LogLevel.Error)]
  [InlineData(LogLevel.Warn)]
  [InlineData(LogLevel.Notice)]
  public void Debug_SkipMessage(LogLevel logLevel)
  {
    // arrange
    var sut = _serviceProvider.ResolveWith<DefaultLogger>(logLevel);

    // act
    sut.Debug(DefaultLoggerTests.Message);

    // assert
    _logAppenderMock.DidNotReceiveWithAnyArgs().WriteLine(default, Arg.Any<string>());
  }

  [Theory]
  [InlineData(LogLevel.Verbose)]
  public void Debug_LogMessage(LogLevel logLevel)
  {
    // arrange
    var sut = _serviceProvider.ResolveWith<DefaultLogger>(logLevel);

    // act
    sut.Debug(DefaultLoggerTests.Message);

    // assert
    _logAppenderMock.Received(1)
      .WriteLine(LogLevel.Verbose, $"{DefaultLoggerTests.HeaderLogLevelVerbose}{DefaultLoggerTests.Message}");
  }

  [Theory]
  [InlineData(LogLevel.Silent)]
  [InlineData(LogLevel.Error)]
  [InlineData(LogLevel.Warn)]
  public void Log_SkipMessage(LogLevel logLevel)
  {
    // arrange
    var sut = _serviceProvider.ResolveWith<DefaultLogger>(logLevel);

    // act
    sut.Log(DefaultLoggerTests.Message);

    // assert
    _logAppenderMock.DidNotReceiveWithAnyArgs().WriteLine(default, Arg.Any<string>());
  }

  [Theory]
  [InlineData(LogLevel.Notice)]
  [InlineData(LogLevel.Verbose)]
  public void Log_LogMessage(LogLevel logLevel)
  {
    // arrange
    var sut = _serviceProvider.ResolveWith<DefaultLogger>(logLevel);

    // act
    sut.Log(DefaultLoggerTests.Message);

    // assert
    _logAppenderMock.Received(1)
      .WriteLine(LogLevel.Notice, $"{DefaultLoggerTests.HeaderLogLevelNotice}{DefaultLoggerTests.Message}");
  }

  [Theory]
  [InlineData(LogLevel.Silent)]
  [InlineData(LogLevel.Error)]
  public void Warn_SkipMessage(LogLevel logLevel)
  {
    // arrange
    var sut = _serviceProvider.ResolveWith<DefaultLogger>(logLevel);

    // act
    sut.Warn(DefaultLoggerTests.Message);

    // assert
    _logAppenderMock.DidNotReceiveWithAnyArgs().WriteLine(default, Arg.Any<string>());
  }

  [Theory]
  [InlineData(LogLevel.Warn)]
  [InlineData(LogLevel.Notice)]
  [InlineData(LogLevel.Verbose)]
  public void Warn_LogMessage(LogLevel logLevel)
  {
    // arrange
    var sut = _serviceProvider.ResolveWith<DefaultLogger>(logLevel);

    // act
    sut.Warn(DefaultLoggerTests.Message);

    // assert

    _logAppenderMock.Received(1)
      .WriteLine(LogLevel.Warn, $"{DefaultLoggerTests.HeaderLogLevelWarn}{DefaultLoggerTests.Message}");
  }

  [Theory]
  [InlineData(LogLevel.Silent)]
  public void Error_SkipMessage(LogLevel logLevel)
  {
    // arrange
    var sut = _serviceProvider.ResolveWith<DefaultLogger>(logLevel);

    // act
    sut.Error(DefaultLoggerTests.Message);

    // assert
    _logAppenderMock.DidNotReceiveWithAnyArgs().WriteLine(default, Arg.Any<string>());
  }

  [Theory]
  [InlineData(LogLevel.Error)]
  [InlineData(LogLevel.Warn)]
  [InlineData(LogLevel.Notice)]
  [InlineData(LogLevel.Verbose)]
  public void Error_LogMessage(LogLevel logLevel)
  {
    // arrange
    var sut = _serviceProvider.ResolveWith<DefaultLogger>(logLevel);

    // act
    sut.Error(DefaultLoggerTests.Message);

    // assert
    _logAppenderMock.Received(1)
      .WriteLine(LogLevel.Error, $"{DefaultLoggerTests.HeaderLogLevelError}{DefaultLoggerTests.Message}");
  }

  [Theory]
  [InlineData("{0}", "1", 1)]
  [InlineData("{0},{1}", "1,2", 1, 2)]
  [InlineData("{0},{1},{2}", "1,2,3", 1, 2, 3)]
  public void Error_GivenParameters_LogFormattedMessage(string messageFormat, string expectedMessage,
    params object[] parameters)
  {
    // arrange
    var sut = _serviceProvider.ResolveWith<DefaultLogger>(LogLevel.Verbose);

    // act
    sut.Error(messageFormat, parameters);

    // assert
    _logAppenderMock.Received(1)
      .WriteLine(LogLevel.Error, $"{DefaultLoggerTests.HeaderLogLevelError}{expectedMessage}");
  }
}
