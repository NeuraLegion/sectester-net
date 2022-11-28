using System;
using System.Collections.Generic;

namespace SecTester.Repeater.Runners;

public record RequestRunnerOptions
{
  public TimeSpan? Timeout { get; init; } = TimeSpan.FromSeconds(30);

  public Uri? ProxyUrl { get; init; }

  public IEnumerable<KeyValuePair<string, IEnumerable<string>>>? Headers { get; init; }

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

  public int? MaxContentLength { get; init; } = 1024;

  public bool? ReuseConnection { get; init; } = false;
}
