using System;
using SecTester.Core.Logger;

namespace SecTester.Core.Extensions;

public static class LoggerExtensions
{
  public static void Error(this ILogger logger, Exception e)
  {
    logger.Error(e.ToString());
  }
}
