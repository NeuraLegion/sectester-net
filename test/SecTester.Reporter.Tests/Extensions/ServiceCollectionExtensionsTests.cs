using SecTester.Reporter.Extensions;

namespace SecTester.Reporter.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
  private readonly ServiceCollection _services;

  public ServiceCollectionExtensionsTests()
  {
    _services = new ServiceCollection();
  }

  [Fact]
  public void AddSecTesterReporter_ReturnsDefaultFormatter()
  {
    // act
    _services.AddSecTesterReporter();

    // assert
    using var provider = _services.BuildServiceProvider();
    var result = provider.GetRequiredService<Formatter>();
    result.Should().BeOfType<DefaultFormatter>();
  }
}
