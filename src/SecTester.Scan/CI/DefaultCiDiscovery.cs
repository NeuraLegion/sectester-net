namespace SecTester.Scan.CI;

// TODO (dmitry.osrikov@brightsec.com): rework using 'ci-info' compatible implementation
internal class DefaultCiDiscovery : CiDiscovery
{
  public CiServer Server { get; } = CiServer.Unknown;

  public bool IsCi => Server != CiServer.Unknown;

  public bool IsPr { get; }
}
