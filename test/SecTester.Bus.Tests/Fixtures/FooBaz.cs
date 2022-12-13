namespace SecTester.Bus.Tests.Fixtures;

internal record FooBaz(IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers);
