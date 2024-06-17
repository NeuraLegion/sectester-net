using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace SecTester.Core.Internal;

public static class HttpMethods
{
  public static IDictionary<string, HttpMethod> Items { get; } = typeof(HttpMethod)
      .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
      .Where(x => x.PropertyType.IsAssignableFrom(typeof(HttpMethod)))
      .Select(x => x.GetValue(null))
      .Cast<HttpMethod>()
      .Concat(new List<HttpMethod>
      {
            new("PATCH"),
            new("COPY"),
            new("LINK"),
            new("UNLINK"),
            new("PURGE"),
            new("LOCK"),
            new("UNLOCK"),
            new("PROPFIND"),
            new("VIEW")
      })
      .Distinct()
      .ToDictionary(x => x.Method, x => x, StringComparer.InvariantCultureIgnoreCase);
}
