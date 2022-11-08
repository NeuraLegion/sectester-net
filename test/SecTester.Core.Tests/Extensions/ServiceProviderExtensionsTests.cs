using SecTester.Core.Extensions;

namespace SecTester.Core.Tests.Extensions;

public class ServiceProviderExtensionsTests
{
  private enum Level
  {
    One,
    Two
  }

  private class Fixture
  {
    public Level Level
    {
      get;
      private set;
    }

    public Fixture(Level level = Level.One)
    {
      Level = level;
    }
  }

  [Fact]
  public void ResolveWith_GivenEmptyParameters_ReturnInstanceWithDefaults()
  {
    // arrange
    using var provider = new ServiceCollection()
      .AddTransient<Fixture>()
      .BuildServiceProvider();

    // act
    var result = provider.ResolveWith<Fixture>();

    // assert
    result.Should().BeOfType<Fixture>();
    result.Level.Should().Be(Level.One);
  }

  [Fact]
  public void ResolveWith_GivenParameters_ReturnInstance()
  {
    // arrange
    using var provider = new ServiceCollection()
      .AddTransient<Fixture>()
      .BuildServiceProvider();

    // act
    var result = provider.ResolveWith<Fixture>(Level.Two);

    // assert
    result.Should().BeOfType<Fixture>();
    result.Level.Should().Be(Level.Two);
  }
}
