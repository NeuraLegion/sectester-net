using System;

namespace SecTester.Core.Exceptions;

public class SecTesterException : Exception
{
  public SecTesterException(string message)
    : base(message)
  {
  }
}
