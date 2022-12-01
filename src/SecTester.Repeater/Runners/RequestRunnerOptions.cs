using System;
using System.Collections.Generic;

namespace SecTester.Repeater.Runners;

public record RequestRunnerOptions
{
  /// <summary>
  /// Time to wait for a server to send response headers (and start the response body) before aborting the request.
  /// </summary>
  public TimeSpan? Timeout { get; init; } = TimeSpan.FromSeconds(30);

  /// <summary>
  /// SOCKS4 or SOCKS5 URL to proxy all traffic.
  /// </summary>
  public Uri? ProxyUrl { get; init; }

  /// <summary>
  /// The headers, which is initially empty and consists of zero or more name and value pairs.
  /// </summary>
  public IEnumerable<KeyValuePair<string, IEnumerable<string>>>? Headers { get; init; }

  /// <summary>
  /// The list of allowed mimes, the HTTP response content will be truncated up <see cref="MaxContentLength"/> if its content type does not consist in the list.
  /// </summary>
  public IEnumerable<string>? AllowedMimes { get; init; } = new List<string>
  {
    "text/html",
    "text/plain",
    "text/css",
    "text/javascript",
    "text/markdown",
    "text/xml",
    "application/javascript",
    "application/x-javascript",
    "application/json",
    "application/xml",
    "application/x-www-form-urlencoded",
    "application/msgpack",
    "application/ld+json",
    "application/graphql"
  };

  /// <summary>
  /// The max size of the HTTP response content in bytes allowed for mimes different from <see cref="AllowedMimes"/>.
  /// </summary>
  public int? MaxContentLength { get; init; } = 1024;

  /// <summary>
  /// Configure experimental support for TCP connections reuse.
  /// </summary>
  public bool? ReuseConnection { get; init; } = false;
}
