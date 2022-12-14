using System.Collections.Generic;

namespace SecTester.Repeater.Runners;

public interface IResponse
{
  int? StatusCode { get; init; }
  string? Body { get; init; }
  string? Message { get; init; }
  string? ErrorCode { get; init; }
  Protocol Protocol { get; init; }
  IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; init; }
}
