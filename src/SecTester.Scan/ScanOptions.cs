using System;

namespace SecTester.Scan;

public record ScanOptions(TimeSpan? Timeout = default, TimeSpan? PollingInterval = default,
  bool? DeleteOnDispose = default);
