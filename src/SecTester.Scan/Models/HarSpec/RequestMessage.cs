using System.Collections.Generic;

namespace SecTester.Scan.Models.HarSpec;

public record RequestMessage : EntryMessage
{
  public string? Method { get; init; }
  public string? Url { get; init; }
  public PostData? PostData { get; init; }
  public IEnumerable<QueryParameter> QueryString { get; init; } = new List<QueryParameter>();
}
