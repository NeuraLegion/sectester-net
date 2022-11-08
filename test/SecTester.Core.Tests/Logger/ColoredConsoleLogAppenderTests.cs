using SecTester.Core.Logger;

namespace SecTester.Core.Tests.Logger;

[Collection("Sequential")]
public class ColoredConsoleLogAppenderTests
{
  [Theory]
  [InlineData("")]
  [InlineData("message")]
  public void WriteLine_AppendCRLF(string message)
  {
    // arrange
    var outStringWriter = new StringWriter();
    Console.SetOut(outStringWriter);

    var sut = new ColoredConsoleLogAppender();

    // act
    sut.WriteLine(LogLevel.Verbose, message);

    //assert
    outStringWriter.ToString().Should().Be(message + Environment.NewLine);
  }

  [Theory]
  [InlineData(LogLevel.Notice)]
  [InlineData(LogLevel.Verbose)]
  [InlineData(LogLevel.Warn)]
  public void WriteLine_WriteToSTDOUT(LogLevel logLevel)
  {
    // arrange
    var message = "message";
    var outStringWriter = new StringWriter();
    var errStringWriter = new StringWriter();

    Console.SetOut(outStringWriter);
    Console.SetError(errStringWriter);

    var sut = new ColoredConsoleLogAppender();

    // act
    sut.WriteLine(logLevel, message);

    //assert
    outStringWriter.ToString().Should().StartWith(message);
    errStringWriter.ToString().Should().BeEmpty();
  }

  [Theory]
  [InlineData(LogLevel.Error)]
  public void WriteLine_WriteToSTDERR(LogLevel logLevel)
  {
    // arrange
    var message = "message";
    var outStringWriter = new StringWriter();
    var errStringWriter = new StringWriter();

    Console.SetOut(outStringWriter);
    Console.SetError(errStringWriter);

    var sut = new ColoredConsoleLogAppender();

    // act
    sut.WriteLine(logLevel, message);

    //assert
    outStringWriter.ToString().Should().BeEmpty();
    errStringWriter.ToString().Should().StartWith(message);
  }
}
