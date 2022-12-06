using System;

namespace SecTester.Scan.Target.Har;

public record Entry(DateTime StartedDateTime)
{
  public int Time { get; init; }
  public Timings? Timings { get; init; }
  public Cache? Cache { get; init; }
  public Request? Request { get; init; }
  public Response? Response { get; init; }
};
