using System.Collections.Generic;

namespace SecTester.Scan.Target.HarSpec;

public record Log(Tool Creator)
{
  public IEnumerable<Entry> Entries { get; init; } = new List<Entry>();
  public string Version { get; init; } = "1.2";
}
