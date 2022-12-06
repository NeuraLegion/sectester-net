using System;

namespace SecTester.Scan.Target.Har;

public record Entry(DateTime StartedDateTime, Request Request, Response Response, Timings Timings, Cache Cache)
{
  public int Time { get; init; } = 0;
};
