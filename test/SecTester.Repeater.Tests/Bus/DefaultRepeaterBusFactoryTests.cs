namespace SecTester.Repeater.Tests.Bus;

public class DefaultRepeaterBusFactoryTests : IDisposable
{
  private const string Hostname = "app.brightsec.com";
  private const string Token = "0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk";

  private readonly ILoggerFactory _loggerFactory = Substitute.For<ILoggerFactory>();
  private readonly ITimerProvider _timerProvider = Substitute.For<ITimerProvider>();
  private readonly IServiceScopeFactory _serviceScopeFactory = Substitute.For<IServiceScopeFactory>();

  public DefaultRepeaterBusFactoryTests()
  {
    // ADHOC: since GetRequiredService is part of extension we should explicitly mock an instance method
    _serviceScopeFactory.CreateAsyncScope().ServiceProvider.GetService(typeof(ITimerProvider)).Returns(_timerProvider);
  }

  public void Dispose()
  {
    _timerProvider.ClearSubstitute();
    _serviceScopeFactory.ClearSubstitute();
    _loggerFactory.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task Create_CreatesBus()
  {
    // arrange
    Configuration config = new(Hostname, new Credentials(Token));
    DefaultRepeaterBusFactory sut = new(config, _loggerFactory, _serviceScopeFactory);

    // act
    await using var bus = sut.Create();

    // assert
    bus.Should().BeAssignableTo<IRepeaterBus>();
  }

  [Fact]
  public async Task Create_CredentialsNotDefined_ThrowsError()
  {
    // arrange
    Configuration config = new(Hostname);
    DefaultRepeaterBusFactory sut = new(config, _loggerFactory, _serviceScopeFactory);

    // act
    var act = async () =>
    {
      await using var _ = sut.Create();
    };

    // assert
    await act.Should().ThrowAsync<InvalidOperationException>();
  }
}
