using System.Collections.Generic;
using System.Net.Http;
using SecTester.Scan.Models;

namespace SecTester.Scan.Target;

public interface TargetOptions
{
  /// <summary>
  ///  The server URL that will be used for the request
  /// </summary>
  string Url { get; set; }

  /// <summary>
  /// The query parameters to be sent with the request
  /// </summary>
  Dictionary<string, string>? Query { get; set; }

  /// <summary>
  /// The data to be sent as the request body.
  /// The only required for POST, PUT, PATCH, and DELETE
  /// </summary>
  HttpContent? Body { get; set; }

  /// <summary>
  /// The request method to be used when making the request, GET by default
  /// </summary>
  RequestMethod? Method { get; set; }

  /// <summary>
  /// The headers
  /// </summary>
  Dictionary<string, string>? Headers { get; set; }
}
