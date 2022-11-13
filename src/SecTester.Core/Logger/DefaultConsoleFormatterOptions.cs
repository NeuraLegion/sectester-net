using Microsoft.Extensions.Logging.Console;

namespace SecTester.Core.Logger;

public class DefaultConsoleFormatterOptions: ConsoleFormatterOptions
{
  public DefaultConsoleFormatterOptions()
  {
    TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
    UseUtcTimestamp = false;
    IncludeScopes = false;
  }
}
