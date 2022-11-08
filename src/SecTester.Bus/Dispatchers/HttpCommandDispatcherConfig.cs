using System;

namespace SecTester.Bus.Dispatchers;

public record HttpCommandDispatcherConfig(string BaseUrl, string Token, TimeSpan? Timeout = null)
{
  public string BaseUrl { get; } = BaseUrl;
  public string Token { get; } = Token;
  public TimeSpan? Timeout { get; } = Timeout;
}
