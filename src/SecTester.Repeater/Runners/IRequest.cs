using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SecTester.Repeater.Runners;

public interface IRequest
{
  string? Body { get; set; }
  HttpMethod Method { get; set; }
  Protocol Protocol { get; set; }
  Uri Url { get; set; }
  IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; }
}
