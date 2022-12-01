using SecTester.Repeater.Tests.Mocks;

namespace SecTester.Repeater.Tests;

public class DefaultRepeaterFactoryTests : IDisposable
{
  private const string Id = "99138d92-69db-44cb-952a-1cd9ec031e20";
  private const string DefaultNamePrefix = "sectester";
  private const string Hostname = "app.neuralegion.com";

  private readonly IServiceScopeFactory _serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
  private readonly EventBusFactory _eventBusFactory = Substitute.For<EventBusFactory>();
  private readonly Configuration _configuration = new(Hostname);

  private readonly Repeaters _repeaters = Substitute.For<Repeaters>();
  private readonly MockLogger _logger = Substitute.For<MockLogger>();
  private readonly TimerProvider _timerProvider = Substitute.For<TimerProvider>();
  private readonly DefaultRepeaterFactory _sut;

  public DefaultRepeaterFactoryTests()
  {
    // ADHOC: since GetRequiredService is part of extension we should explicitly mock an instance method
    _serviceScopeFactory.CreateAsyncScope().ServiceProvider.GetService(typeof(TimerProvider)).Returns(_timerProvider);
    _sut = new DefaultRepeaterFactory(_serviceScopeFactory, _repeaters, _eventBusFactory, _configuration, _logger);
  }

  public void Dispose()
  {
    _timerProvider.ClearSubstitute();
    _serviceScopeFactory.ClearSubstitute();
    _eventBusFactory.ClearSubstitute();
    _repeaters.ClearSubstitute();
    _logger.ClearSubstitute();
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
