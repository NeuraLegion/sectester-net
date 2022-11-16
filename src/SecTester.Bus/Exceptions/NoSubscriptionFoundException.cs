using System;

namespace SecTester.Bus.Exceptions;

public class NoSubscriptionFoundException : Exception
{
  public NoSubscriptionFoundException(string eventName) : base(
    $"No subscriptions found. Please register a handler for the {eventName} event in the event bus.")
  {
  }
}
