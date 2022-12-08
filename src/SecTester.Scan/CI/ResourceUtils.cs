using System;
using System.IO;
using System.Reflection;

namespace SecTester.Scan.CI;

internal static class ResourceUtils
{
  public static string GetEmbeddedResourceContent<T>(string resourceName)
    where T : class
  {
    if (string.IsNullOrWhiteSpace(resourceName))
    {
      throw new ArgumentNullException(nameof(resourceName));
    }

    var assembly = typeof(T).GetTypeInfo().Assembly;
    using var stream = assembly.GetManifestResourceStream(resourceName);

    if (stream is null)
    {
      throw new InvalidOperationException($"Could not get stream for {resourceName} resource.");
    }

    using var reader = new StreamReader(stream);

    return reader.ReadToEnd();
  }
}
