namespace SecTester.Core.Logger;

public interface ILogAppender
{
  void WriteLine(LogLevel logLevel, string message);
}
