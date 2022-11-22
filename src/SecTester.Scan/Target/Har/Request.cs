using System.Collections.Generic;

namespace SecTester.Scan.Target.Har;

public record Request : Message
{
  public string Method { get; init; }
  public string Url { get; init; }
  public IEnumerable<QueryParameter> QueryString { get; init; } = new List<QueryParameter>();
  public PostData PostData { get; init; }
}
