namespace SecTester.Scan.CI;

public record CiServer(string? ServerName = default)
{
  public static readonly CiServer Unknown = new();
}
