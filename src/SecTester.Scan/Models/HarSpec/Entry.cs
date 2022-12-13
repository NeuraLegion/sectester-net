using System;

namespace SecTester.Scan.Models.HarSpec;

public record Entry(DateTime StartedDateTime, RequestMessage Request, ResponseMessage Response, Timings Timings, Cache Cache)
{
  public int Time { get; init; }
}
