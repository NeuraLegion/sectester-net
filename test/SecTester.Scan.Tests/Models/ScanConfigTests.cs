#pragma warning disable CS8604

using SecTester.Scan.Models;

namespace SecTester.Scan.Tests.Models;

public class ScanConfigTests
{
  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // arrange
    const string name = "name";
    const Module module = Module.Fuzzer;
    const int poolSize = 20;
    const string fileId = "fileId";
    const bool smart = true;
    const bool skipStaticParams = true;
    const string projectId = "projectId";
    const int slowEpTimeout = 10;
    const int targetTimeout = 5;

    var tests = new TestType[] { };
    var discoveryTypes = new Discovery[] { Discovery.Archive };
    var attackParamLocations = new AttackParamLocation[] { AttackParamLocation.Body };
    var hostsFilter = new string[] { "example.com" };
    var repeaters = new string[] { "repeater" };

    // act
    var scanConfig = new ScanConfig(name, module, tests,
      discoveryTypes, poolSize,
      attackParamLocations, fileId, hostsFilter,
      repeaters, smart, skipStaticParams, projectId,
      slowEpTimeout, targetTimeout);

    // assert
    scanConfig.Module.Should().Be(module);
    scanConfig.Tests.Should().BeEquivalentTo(tests);
    scanConfig.DiscoveryTypes.Should().BeEquivalentTo(discoveryTypes);
    scanConfig.PoolSize.Should().Be(poolSize);
    scanConfig.AttackParamLocations.Should().BeEquivalentTo(attackParamLocations);
    scanConfig.FileId.Should().Be(fileId);
    scanConfig.HostsFilter.Should().BeEquivalentTo(hostsFilter);
    scanConfig.Repeaters.Should().BeEquivalentTo(repeaters);
    scanConfig.Smart.Should().Be(smart);
    scanConfig.SkipStaticParams.Should().Be(skipStaticParams);
    scanConfig.ProjectId.Should().Be(projectId);
    scanConfig.SlowEpTimeout.Should().Be(slowEpTimeout);
    scanConfig.TargetTimeout.Should().Be(targetTimeout);
  }

  [Fact]
  public void Constructor_GivenNullName_ThrowError()
  {
    // act
    Action act = () => new ScanConfig(null as string);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }
}
