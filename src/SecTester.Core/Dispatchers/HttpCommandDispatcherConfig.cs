using System;

namespace SecTester.Core.Dispatchers;

public sealed record HttpCommandDispatcherConfig(string BaseUrl, string Token, TimeSpan? Timeout = null);
