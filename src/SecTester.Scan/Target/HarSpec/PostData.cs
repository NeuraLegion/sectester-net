using System.Collections.Generic;

namespace SecTester.Scan.Target.HarSpec;

public record PostData
{
  public string? MimeType { get; init; }
  public string? Text { get; init; }
  public IEnumerable<PostDataParameter> Params { get; init; } = new List<PostDataParameter>();
}
