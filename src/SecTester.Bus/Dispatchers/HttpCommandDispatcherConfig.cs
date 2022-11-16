using System;

namespace SecTester.Bus.Dispatchers;

public sealed record HttpCommandDispatcherConfig(string BaseUrl, string Token, TimeSpan? Timeout = null);
