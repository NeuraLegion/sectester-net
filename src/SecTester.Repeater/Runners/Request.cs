using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace SecTester.Repeater.Runners;

public interface Request
{
  string? Body { get; init; }
  Regex? CorrelationIdRegex { get; init; }
  HttpMethod Method { get; init; }
  Protocol Protocol { get; init; }
  Uri Url { get; init; }
  IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; init; }
}
