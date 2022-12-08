using System;

namespace SecTester.Scan.Models.HarSpec;

public record Entry(DateTime StartedDateTime, RequestMessage RequestMessage, ResponseMessage ResponseMessage, Timings Timings, Cache Cache)
{
  public int Time { get; init; }
}
