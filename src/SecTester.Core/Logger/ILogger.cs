namespace SecTester.Core.Logger;

public interface ILogger
{
  LogLevel LogLevel { get; set; }
  void Error(string message, params object[] args);
  void Warn(string message, params object[] args);
  void Log(string message, params object[] args);
  void Debug(string message, params object[] args);
}
