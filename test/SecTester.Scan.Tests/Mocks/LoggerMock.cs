namespace SecTester.Scan.Tests.Mocks;

public abstract class LoggerMock : ILogger
{
  void ILogger.Log<TState>(
    LogLevel logLevel,
    EventId eventId,
    TState state,
    Exception? exception,
    Func<TState, Exception?, string> formatter)
  {
    Log(logLevel, formatter(state, exception));
  }

  public virtual bool IsEnabled(LogLevel logLevel) => true;

  public abstract IDisposable BeginScope<TState>(TState state);

  public abstract void Log(LogLevel logLevel, string message);
}
