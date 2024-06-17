using SecTester.Repeater.Extensions;

namespace SecTester.Scan.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
  private readonly Configuration _config;
  private readonly ServiceCollection _services;

  public ServiceCollectionExtensionsTests()
  {
    _services = new ServiceCollection();
    _config = new Configuration("app.brightsec.com",
      new Credentials("0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk"));
  }

  [Fact]
  public void AddSecTesterScan_ReturnsDefaultScans()
  {
    // arrange
    _services.AddSecTesterConfig(_config);
    _services.AddSecTesterRepeater();

    // act
    _services.AddSecTesterScan();

    // assert
    using var provider = _services.BuildServiceProvider();
    var result = provider.GetRequiredService<IScans>();
    result.Should().BeOfType<DefaultScans>();
  }

  [Fact]
  public void AddSecTesterScan_ReturnsCiDiscovery()
  {
    // arrange
    _services.AddSecTesterConfig(_config);
    _services.AddSecTesterRepeater();

    // act
    _services.AddSecTesterScan();

    // assert
    using var provider = _services.BuildServiceProvider();
    var result = provider.GetRequiredService<CiDiscovery>();
    result.Should().BeOfType<DefaultCiDiscovery>();
  }

  [Fact]
  public void AddSecTesterScan_ReturnsScanFactory()
  {
    // arrange
    _services.AddSecTesterConfig(_config);
    _services.AddSecTesterRepeater();

    // act
    _services.AddSecTesterScan();

    // assert
    using var provider = _services.BuildServiceProvider();
    var result = provider.GetRequiredService<IScanFactory>();
    result.Should().BeOfType<DefaultScanFactory>();
  }
}
