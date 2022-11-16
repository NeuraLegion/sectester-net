namespace SecTester.Bus.RetryStrategies;

public record ExponentialBackoffOptions(int MaxDepth = 3, int MinInterval = 50);
