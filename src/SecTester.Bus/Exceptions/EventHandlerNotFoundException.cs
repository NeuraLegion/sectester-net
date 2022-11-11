using System;

namespace SecTester.Bus.Exceptions;

public class EventHandlerNotFoundException : Exception
{
  public EventHandlerNotFoundException(params string[] eventNames) : base(
    $"Event handler not found. Please register a handler for the following events: {string.Join(", ", eventNames)}")
  {
  }
}
