namespace SecTester.Scan.Tests.Extensions;

internal static class HttpContentExtensions
{
  public static string? ReadHttpContentAsString(this HttpContent? content)
  {
    return content is null
      ? default
      : Task.Run(content.ReadAsStringAsync).GetAwaiter().GetResult();
  }
}
