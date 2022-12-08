namespace SecTester.Scan.CI;

// TODO (dmitry.osrikov@brightsec.com): rework using 'ci-info' compatible implementation
internal class DefaultCiDiscovery : CiDiscovery
{
  public CiServer? Server { get; }

  public bool IsCi => Server != null;
}
