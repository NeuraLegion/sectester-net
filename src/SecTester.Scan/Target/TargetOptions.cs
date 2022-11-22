using System.Collections.Generic;
using System.Net.Http;

namespace SecTester.Scan.Target;

public interface TargetOptions
{
  /// <summary>
  ///   The server URL that will be used for the request
  /// </summary>
  string Url { get; }

  /// <summary>
  ///   The query parameters to be sent with the request
  /// </summary>
  IEnumerable<KeyValuePair<string, string>>? Query { get; }

  /// <summary>
  ///   The data to be sent as the request body.
  ///   The only required for POST, PUT, PATCH, and DELETE
  /// </summary>
  HttpContent? Body { get; }

  /// <summary>
  ///   The request method to be used when making the request, GET by default
  /// </summary>
  HttpMethod? Method { get; }

  /// <summary>
  ///   The headers
  /// </summary>
  IEnumerable<KeyValuePair<string, IEnumerable<string>>>? Headers { get; }

  /// <summary>
  ///   The optional method of serializing `Query`
  /// </summary>
  string SerializeQuery(IEnumerable<KeyValuePair<string, string>> pairs);
}
