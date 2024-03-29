namespace SecTester.Repeater.Tests;

public class DefaultRepeaterFactoryTests : IDisposable
{
  private const string Id = "99138d92-69db-44cb-952a-1cd9ec031e20";
  private const string DefaultNamePrefix = "sectester";
  private const string Hostname = "app.brightsec.com";

  private readonly IRepeaterBusFactory _busFactory = Substitute.For<IRepeaterBusFactory>();
  private readonly Configuration _configuration = new(Hostname);

  private readonly IRepeaters _repeaters = Substitute.For<IRepeaters>();
  private readonly ILoggerFactory _loggerFactory = Substitute.For<ILoggerFactory>();
  private readonly IAnsiCodeColorizer _ansiCodeColorizer = Substitute.For<IAnsiCodeColorizer>();
  private readonly RequestRunnerResolver _resolver = Substitute.For<RequestRunnerResolver>();

  private readonly DefaultRepeaterFactory _sut;

  public DefaultRepeaterFactoryTests()
  {
    _sut = new DefaultRepeaterFactory(_repeaters, _busFactory, _configuration, _loggerFactory, _ansiCodeColorizer, _resolver);
  }

  public void Dispose()
  {
    _ansiCodeColorizer.ClearSubstitute();
    _busFactory.ClearSubstitute();
    _repeaters.ClearSubstitute();
    _loggerFactory.ClearSubstitute();
    _resolver.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task CreateRepeater_CreatesRepeater()
  {
    // arrange
    _repeaters.CreateRepeater(Arg.Is<string>(s => s.Contains(DefaultNamePrefix)))
      .Returns(Id);

    // act
    await using var repeater = await _sut.CreateRepeater();

    // assert
    repeater.Should().BeOfType<Repeater>();
    repeater.Should().BeEquivalentTo(
      new
      {
        RepeaterId = Id
      });
  }

  [Fact]
  public async Task CreateRepeater_GivenOptions_CreatesRepeater()
  {
    // arrange
    var options = new RepeaterOptions
    {
      NamePrefix = "foo",
      Description = "bar"
    };
    _repeaters.CreateRepeater(Arg.Is<string>(s => s.Contains(options.NamePrefix)), options.Description)
      .Returns(Id);

    // act
    await using var repeater = await _sut.CreateRepeater(options);

    // assert
    repeater.Should().BeOfType<Repeater>();
    repeater.Should().BeEquivalentTo(
      new
      {
        RepeaterId = Id
      });
  }
}
