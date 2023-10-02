using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SecTester.Repeater.Runners;

public interface IRequest
{
  string? Body { get; init; }
  HttpMethod Method { get; init; }
  Protocol Protocol { get; init; }
  Uri Url { get; init; }
  IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; init; }
}
