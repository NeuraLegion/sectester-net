namespace SecTester.Scan.Tests.Fixtures;

public record TestTargetOptions: TargetOptions
{
  public string Url { get; init; }
  public IEnumerable<KeyValuePair<string, string>>? Query { get; init;}
  public HttpContent? Body { get; init;}
  public HttpMethod? Method { get; init;}
  public IEnumerable<KeyValuePair<string, IEnumerable<string>>>? Headers { get; init;}
  public string SerializeQuery(IEnumerable<KeyValuePair<string, string>> pairs)
  {
    return string.Join('&', pairs.Select(x => $"{x.Key}={x.Value}"));
  }
}
