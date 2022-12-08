namespace SecTester.Scan.CI;

public interface CiDiscovery
{
  CiServer? Server { get; }
  bool IsCi { get; }
  bool IsPr { get; }
}
