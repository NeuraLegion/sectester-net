using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SecTester.Scan.Target;

public interface TargetOptions
{
  /// <summary>
  ///   The server URL that will be used for the request
  /// </summary>
  string Url { get; init; }

  /// <summary>
  ///   The query parameters to be sent with the request
  /// </summary>
  IEnumerable<KeyValuePair<string, string>>? Query { get; init; }

  /// <summary>
  ///   The data to be sent as the request body.
  ///   The only required for POST, PUT, PATCH, and DELETE
  /// </summary>
  HttpContent? Body { get; init; }

  /// <summary>
  ///   The request method to be used when making the request, GET by default
  /// </summary>
  HttpMethod? Method { get; set; }

  /// <summary>
  ///   The headers
  /// </summary>
  IEnumerable<KeyValuePair<string, IEnumerable<string>>>? Headers { get; init; }

  /// <summary>
  /// The optional method of serializing `Query`
  /// </summary>
  Func<IDictionary<string, IEnumerable<string>>, string>? SerializeQuery { get; init; }
}
