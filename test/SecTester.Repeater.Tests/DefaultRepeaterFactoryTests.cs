namespace SecTester.Repeater.Tests;

public class DefaultRepeaterFactoryTests : IDisposable
{
  private const string Id = "99138d92-69db-44cb-952a-1cd9ec031e20";
  private const string DefaultNamePrefix = "sectester";
  private const string Hostname = "app.brightsec.com";

  private readonly IServiceScopeFactory _serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
  private readonly IRepeaterEventBusFactory _eventBusFactory = Substitute.For<IRepeaterEventBusFactory>();
  private readonly Configuration _configuration = new(Hostname);

  private readonly IRepeaters _repeaters = Substitute.For<IRepeaters>();
  private readonly ILoggerFactory _loggerFactory = Substitute.For<ILoggerFactory>();
  private readonly ITimerProvider _timerProvider = Substitute.For<ITimerProvider>();
  private readonly IAnsiCodeColorizer _ansiCodeColorizer = Substitute.For<IAnsiCodeColorizer>();
  private readonly DefaultRepeaterFactory _sut;

  public DefaultRepeaterFactoryTests()
  {
    // ADHOC: since GetRequiredService is part of extension we should explicitly mock an instance method
    _serviceScopeFactory.CreateAsyncScope().ServiceProvider.GetService(typeof(ITimerProvider)).Returns(_timerProvider);
    _sut = new DefaultRepeaterFactory(_serviceScopeFactory, _repeaters, _eventBusFactory, _configuration, _loggerFactory, _ansiCodeColorizer);
  }

  public void Dispose()
  {
    _ansiCodeColorizer.ClearSubstitute();
    _timerProvider.ClearSubstitute();
    _serviceScopeFactory.ClearSubstitute();
    _eventBusFactory.ClearSubstitute();
    _repeaters.ClearSubstitute();
    _loggerFactory.ClearSubstitute();
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
