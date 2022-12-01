namespace SecTester.Repeater.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
  private readonly IServiceCollection _sut = Substitute.ForPartsOf<ServiceCollection>();

  [Fact]
  public void AddSecTesterRepeater_RegistersRepeaterFactory()
  {
    // act
    _sut.AddSecTesterRepeater();

    // assert
    _sut.Received().AddScoped<RepeaterFactory, DefaultRepeaterFactory>();
  }

  [Fact]
  public void AddSecTesterRepeater_RegistersRepeaters()
  {
    // act
    _sut.AddSecTesterRepeater();

    // assert
    _sut.Received().AddScoped<Repeaters, DefaultRepeaters>();
  }

  [Fact]
  public void AddSecTesterRepeater_RegistersTimerProvider()
  {
    // act
    _sut.AddSecTesterRepeater();

    // assert
    _sut.Received().AddScoped<TimerProvider, SystemTimerProvider>();
  }
}
