namespace SecTester.Core.RetryStrategies;

public record ExponentialBackoffOptions(int MaxDepth = 3, int MinInterval = 50);
