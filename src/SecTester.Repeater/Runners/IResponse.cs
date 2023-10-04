using System.Collections.Generic;

namespace SecTester.Repeater.Runners;

public interface IResponse
{
  int? StatusCode { get; set; }
  string? Body { get; set; }
  string? Message { get; set; }
  string? ErrorCode { get; set; }
  Protocol Protocol { get; set; }
  IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; }
}
