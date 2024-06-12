using System;

namespace SecTester.Repeater.Dispatchers;

public sealed record HttpCommandDispatcherConfig(string BaseUrl, string Token, TimeSpan? Timeout = null);
