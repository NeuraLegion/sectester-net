using System;

namespace SecTester.Core.Exceptions;

public class SecTesterException : Exception
{
  public SecTesterException(string message, Exception? innerException = default)
    : base(message, innerException)
  {
  }
}
